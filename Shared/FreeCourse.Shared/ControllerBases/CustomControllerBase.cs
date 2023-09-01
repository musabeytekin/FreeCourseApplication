using System;
using FreeCourse.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Shared.ControllerBases
{
	public class CustomControllerBase : ControllerBase
	{
		public IActionResult CreateActionResultInstance<T>(Response<T> response)
		{
			return new ObjectResult(response)
			{
				StatusCode = response.StatusCode
			};
		}
	}
}

