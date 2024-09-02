namespace Application.Interfaces;

public interface IFileReaderService
{
    List<T> ReadFromFile<T>(string filePath);
}