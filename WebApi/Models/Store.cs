using Google.Cloud.Firestore;

namespace WebApi.Models;

[FirestoreData]
public class Store
{
    [FirestoreDocumentId]
    public string Id { get; set; }

    [FirestoreProperty]
    public string Name { get; set; }
}