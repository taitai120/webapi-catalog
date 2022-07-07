using System.Collections;
using System.Collections.Generic;
using Catalog.Repositories;
using Catalog.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Catalog.Dtos;
using System.Threading.Tasks;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository repository;

        public ItemsController(IItemsRepository _repository)
        {
            repository = _repository;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetItemsAsync()
        {
            var items = (await repository.GetItemsAsync()).Select(item => item.AsDto());

            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
        {
            var findItem = await repository.GetItemAsync(id);

            if (findItem is null)
            {
                return NotFound();
            }

            return findItem.AsDto();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItem(Guid id)
        {

            var findItem = await repository.GetItemAsync(id);

            if (findItem == null)
            {
                return NotFound();
            }

            await repository.DeleteItemAsync(id);

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto)
        {
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await repository.CreateItemAsync(item);

            return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItem(Guid id, UpdateItemDto itemDto)
        {
            var findItem = await repository.GetItemAsync(id);

            if (findItem is null)
            {
                return NotFound();
            }

            Item updateItem = findItem with
            {
                Name = itemDto.Name,
                Price = itemDto.Price
            };

            // Item updateItem = new()
            // {
            //     Id = findItem.Id,
            //     Name = itemDto.Name,
            //     Price = itemDto.Price,
            //     CreatedDate = findItem.CreatedDate
            // };

            await repository.UpdateItemAsync(updateItem);

            return NoContent();
        }
    }
}