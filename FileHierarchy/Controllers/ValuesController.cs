using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FileHierarchy.Models;
using Swashbuckle.Swagger.Annotations;

namespace FileHierarchy.Controllers
{
	public class ValuesController : ApiController
	{
		// GET api/values
		[SwaggerOperation("GetAll")]
		public IEnumerable<string> Get()
		{
			return new string[] { "value1", "value2" };
		}

		// GET api/values/5
		[SwaggerOperation("GetById")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		public string Get(int id)
		{
			var model = new Model();
			var file = model.Entities
				.Include(f => f.Descendants)
				.Single(f => f.Id == id);

			var indexes = file.Descendants
				.OrderBy(d => d.Level)
				.Select(s => s.Parent.SeqNum)
				.ToList();

			indexes.Add(file.SeqNum);
			return string.Join(".", indexes);
		}

		// POST api/values
		[SwaggerOperation("Create")]
		[SwaggerResponse(HttpStatusCode.Created)]
		public void Post([FromBody]string value)
		{
		}

		// PUT api/values/5
		[SwaggerOperation("Update")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/values/5
		[SwaggerOperation("Delete")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		public void Delete(int id)
		{
		}
	}
}
