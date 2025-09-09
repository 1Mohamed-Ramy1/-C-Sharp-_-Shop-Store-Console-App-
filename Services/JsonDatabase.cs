using Newtonsoft.Json;
namespace App.Services;
public interface IIdentifiable
{
    int Id { set; get; }
}
public class JsonDatabase<T> where T : class, IIdentifiable
{
    private readonly string _filePath;
    public JsonDatabase(string filePath)
    {
        _filePath = filePath;
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }
    public List<T> GetAll()
    {
        string json = File.ReadAllText(_filePath);
        List<T> models = JsonConvert.DeserializeObject<List<T>>(json) ?? [];
        return models;
    }
    public void Add(T item)
    {
        var items = GetAll();
        item.Id += items.Count;
        items.Add(item);
        Save(items);
    }
    public T? GetById(int id)
    {
        var items = GetAll();
        foreach (var item in items)
        {
            if (id == item.Id) return item;
        }
        return null;
    }
    public void Update(int id, T data)
    {
        var items = GetAll();
        var item = items.FirstOrDefault(item => item.Id == id);
        if (item == null)
        {
            throw new Exception($"{typeof(T).Name} with id({id}) not found");
        }
        if (id == item.Id)
        {
            T newItem;
            items.Remove(item);
            newItem = data;
            newItem.Id = id;
            items.Add(newItem);
            Save(items);
            return;
        }
    }
    public void Delete(int id)
    {
        var items = GetAll();
        var item = items.FirstOrDefault(item => item.Id == id);

        if (item != null)
        {
            items.Remove(item);
            Save(items);
            return;
        }
        throw new Exception($"{typeof(T).Name} with id({id}) not found");
    }
    private void Save(List<T> items)
    {
        File.WriteAllText(_filePath, JsonConvert.SerializeObject(items, Formatting.Indented));
    }

}