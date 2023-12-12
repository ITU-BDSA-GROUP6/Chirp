using Chirp.Models;
using Chirp.CDTO;
using Chirp.ADTO;
namespace Chirp.Interfaces;

public interface ICheepRepository
{
    public int CheepsPerPage();
    public Task CreateCheep(CheepDTO cheepDTO);
    public Task<IEnumerable<Cheep>> GetCheeps(int page, string orderBy);
    public Task<IEnumerable<Cheep>> GetCheepsFromAuthor(string author, int page);
    public Task<int> GetTotalNumberOfCheeps();
    public Task<int> GetTotalNumberOfAuthorCheeps(string author);
    public Task<IEnumerable<Cheep>> GetTop4FromAuthor(string author);
    public Task GiveOpinionOfCheep(bool IsLike, int CheepId, string AuthorName);
    public Task<Cheep> GetCheepById(int CheepId);
    public Task<bool> DeleteCheep(string author, int CheepId);
    public Task<bool> DeleteAllCheepsFromAuthor(string AuthorUserName);
}