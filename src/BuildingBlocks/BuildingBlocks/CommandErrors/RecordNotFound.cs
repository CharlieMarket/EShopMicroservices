using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.CommandErrors
{
	public class RecordNotFound
	{
		public Guid Id { get; }

		public RecordNotFound(Guid id)
		{
			Id = id;
		}
	}
}
