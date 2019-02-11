using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NICE.Identity.Authorisation.WebAPI.Models.Requests;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
	public interface IClaimsService
	{
		Models.Responses.Claim[] GetClaims(int userId);
	}

	public class ClaimsService
	{
		public Models.Responses.Claim[] GetClaims(int userId)
		{

			throw new NotImplementedException();
		}
	}
}
