using Google.Cloud.Firestore;

namespace WebApi.Models;

[FirestoreData]
public class Product
{
    [FirestoreDocumentId]
    public string Id { get; set; }

    [FirestoreProperty]
    public string Name { get; set; }

    [FirestoreProperty]
    public double Price { get; set; }
}