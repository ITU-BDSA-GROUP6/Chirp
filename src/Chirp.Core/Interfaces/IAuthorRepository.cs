using Chirp.Models;
namespace Chirp.Interfaces;

public interface IAuthorRepository
{
    public void CreateNewAuthor(Author author);
    public Task<Author> GetAuthorFromName(string name); 
}