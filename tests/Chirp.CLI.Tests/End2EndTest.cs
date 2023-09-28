using System;
using System.Globalization;
using System.Diagnostics;
using CsvHelper;
using Chirp.CLI;
using System.IO;
using System.Runtime.InteropServices;


public class End2End
{


    [Fact]
    public void Test_That_A_Cheep_Is_Stored_As_Expected() //E2E test to test the cheep command.
    {
        // Arrange
        // Act
        using (var process = new Process())
        {
            process.StartInfo.FileName = dotNetPath();
            process.StartInfo.Arguments = "bin/Debug/net7.0/Chirp.CLI.dll cheep \"this is a cheep for testing E2E cheeping\""; //The cheep msg
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = "../../../../../src/Chirp.CLI.Client/";
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            // Synchronously read the standard output of the spawned process.
            process.WaitForExit();
                    }

        string output = "";
        List<Cheep> cheepRecords = new List<Cheep>();
        using (StreamReader reader = new StreamReader("../../../../../data/chirp_cli_db.csv"))
        using (CsvReader csvReader = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            cheepRecords = csvReader.GetRecords<Cheep>().ToList();
            output = UserInterface.GetOutputString(cheepRecords[cheepRecords.Count()-1]); //Read the last record out into output string.
        }

        // Assert
        System.Console.WriteLine(output);
        Assert.EndsWith("t", output.Trim()); //Trim to ensure that trailing whitespace does not interfere with the test.

        RemoveLastCheep(); //Cleanup step
    }

    [Fact]
    public void Test_Read_Cheep_Limit_1() //E2E test to test the read command
    {
        // Arrange
        using (StreamWriter writer = File.AppendText("../../../../../src/Chirp.CSVDBService/data/chirp_cli_db.csv"))
        using (CsvWriter csv = new CsvWriter(writer , CultureInfo.InvariantCulture))
        {
            csv.NextRecord();
            csv.WriteRecord(new Cheep("mikto","this is a cheep for testing E2E reading",1694520339)); //A cheep is added to the csv file, to be read in the test.
        }
        // Act
        string output = "";
        using (var process = new Process())
        {
            process.StartInfo.FileName = dotNetPath();
            process.StartInfo.Arguments = "bin/Debug/net7.0/Chirp.CLI.dll --read 1"; //The read command with limit 1
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = "../../../../../src/Chirp.CLI.Client/";
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            StreamReader reader = process.StandardOutput;
            output = reader.ReadToEnd();
            process.WaitForExit();
        }
        // Assert
        Assert.StartsWith("mikto" , output.Trim());
        Assert.EndsWith("this is a cheep for testing E2E reading" , output.Trim());

        RemoveLastCheep(); //Cleanup step
    }

    //A cleanup step that removes the cheep created by the tests.
    private void RemoveLastCheep() //This method is written by chat.openai.com
    {
        string csvFilePath = "../../../../../data/chirp_cli_db.csv";

        // Read all records from the CSV file
        List<Cheep> cheepRecords = new List<Cheep>();
        using (StreamReader reader = new StreamReader(csvFilePath))
        using (CsvReader csvReader = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            cheepRecords = csvReader.GetRecords<Cheep>().ToList();
        }

        // Check if there are records to delete
        if (cheepRecords.Count > 0)
        {
            // Remove the last cheep record
            cheepRecords.RemoveAt(cheepRecords.Count - 1);

            // Write the modified data back to the CSV file
            using (StreamWriter writer = new StreamWriter(csvFilePath))
            using (CsvWriter csvWriter = new CsvWriter(writer, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csvWriter.WriteRecords(cheepRecords);
            }
        }
        
    }

    //Generate path for dotnetcore based on platform
    private string dotNetPath()
    {   
        // The feature of extracting the runtimeinformation is inspired by stackoverflow
        //https://stackoverflow.com/questions/38790802/determine-operating-system-in-net-core
        string path;
        if(System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
            path = "/usr/local/share/dotnet/dotnet";
        } else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            path = @"C:\program files\dotnet\dotnet";
        } else {
            path = "/usr/bin/dotnet";
        }
        return path;
    }
}