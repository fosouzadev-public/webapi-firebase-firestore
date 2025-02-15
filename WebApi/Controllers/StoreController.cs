using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("api/store")]
public class StoreController(FirestoreDb _database) : ControllerBase
{
    #region Store

    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] Store store)
    {
        DocumentReference storeRef = await _database.Collection(nameof(Store)).AddAsync(store);
        DocumentSnapshot storeSnapshot = await storeRef.GetSnapshotAsync();
        Store storeInserted = storeSnapshot.ConvertTo<Store>();

        return Created($"store/{storeRef.Id}", storeInserted);
    }

    [HttpPut]
    public async Task<IActionResult> EditAsync([FromBody] Store store)
    {
        DocumentReference storeRef = _database.Collection(nameof(Store)).Document(store.Id);
        DocumentSnapshot storeSnapshot = await storeRef.GetSnapshotAsync();

        if (storeSnapshot.Exists == false)
            return NotFound($"Store {store.Id} not found.");

        _ = await storeRef.UpdateAsync(new Dictionary<string, object>
        {
            { nameof(Store.Name), store.Name }
        });

        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] string id)
    {
        DocumentReference storeRef = _database.Collection(nameof(Store)).Document(id);
        DocumentSnapshot storeSnapshot = await storeRef.GetSnapshotAsync();

        if (storeSnapshot.Exists == false)
            return NotFound($"Store {id} not found.");

        Store store = storeSnapshot.ConvertTo<Store>();
        return Ok(store);
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] int pageIndex = 0, int pageSize = 10, string filter = null)
    {
        pageIndex = Math.Abs(pageIndex);
        CollectionReference storeRef = _database.Collection(nameof(Store));

        Query query;

        if (string.IsNullOrWhiteSpace(filter) == false)
            query = storeRef
                .WhereGreaterThanOrEqualTo(nameof(Store.Name), filter)
                .WhereLessThan(nameof(Store.Name), $"{filter}~")
                .Offset(pageIndex * pageSize)
                .Limit(pageSize);
        else
            query = storeRef.Offset(pageIndex * pageSize).Limit(pageSize);

        QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

        IEnumerable<Store> stories = querySnapshot.Documents.Select(a => a.ConvertTo<Store>());

        int totalPages = (int)Math.Ceiling(querySnapshot.Count / (double)pageSize);

        return Ok(new
        {
            Items = stories.ToList(),
            PageIndex = pageIndex,
            PageSize = pageSize,
            MaxPageIndex = totalPages > 0 ? totalPages - 1 : totalPages
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] string id)
    {
        DocumentReference storeRef = _database.Collection(nameof(Store)).Document(id);
        DocumentSnapshot storeSnapshot = await storeRef.GetSnapshotAsync();

        if (storeSnapshot.Exists == false)
            return NotFound($"Store {id} not found.");

        _ = await storeRef.DeleteAsync();

        return NoContent();
    }

    #endregion

    #region Product

    [HttpPost("{id}/product")]
    public async Task<IActionResult> AddAsync([FromRoute]string id, [FromBody] Product product)
    {
        DocumentReference storeRef = _database.Collection(nameof(Store)).Document(id);
        DocumentSnapshot storeSnapshot = await storeRef.GetSnapshotAsync();

        if (storeSnapshot.Exists == false)
            return NotFound($"Store {id} not found.");

        DocumentReference productRef = await storeRef.Collection(nameof(Product)).AddAsync(product);
        DocumentSnapshot productSnapshot = await productRef.GetSnapshotAsync();
        Product productInserted = productSnapshot.ConvertTo<Product>();

        return Created($"store/{storeRef.Id}/product/{productRef.Id}", productInserted);
    }

    [HttpGet("{id}/product")]
    public async Task<IActionResult> GetAsync([FromRoute]string id)
    {
        CollectionReference productRef = _database.Collection($"{nameof(Store)}/{id}/{nameof(Product)}");

        QuerySnapshot productSnapshot = await productRef.GetSnapshotAsync();
        
        return Ok(productSnapshot.Documents.Select(a => a.ConvertTo<Product>()));
    }

    #endregion
}