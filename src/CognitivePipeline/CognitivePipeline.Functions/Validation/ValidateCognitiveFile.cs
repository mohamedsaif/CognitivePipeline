using AutoMapper;
using CognitivePipeline.Functions.Abstractions;
using CognitivePipeline.Functions.Models;
using CognitivePipeline.Functions.Models.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.Validation
{
    public static class ValidateCognitiveFile
    {
        public static CognitiveFile ValidateForSubmission(CognitiveFileDTO file, IUserAccountRepository usersRepo)
        {
            //TODO: Add attribute values validation + user db validation

            var newId = Guid.NewGuid().ToString();
            var result = new CognitiveFile
            {
                Id = newId,
                IsProcessed = false,
                CreatedAt = DateTime.UtcNow,
                FileName = $"{newId}{Path.GetExtension(file.FileName)}",
                IsDeleted = false,
                MediaType = FileMediaType.Image,
                Origin = file.Origin,
                OwnerId = file.OwnerId,
                Status = "Submitted"
            };

            foreach(var step in file.CognitivePipelineActions)
            {
                result.CognitivePipelineActions.Add(new CognitiveStep
                {
                    ServiceType = step.ServiceType,
                    Status = "Processing",
                    Confidence = 0,
                    IsSuccessful = false,
                });
            }

            //TODO: Add mapper configuration for successful automated mapping between the DTO and business objects
            //result.CognitivePipelineActions = Mapper.Map<List<CognitiveStep>>(file.CognitivePipelineActions);

            return result;
        }
    }
}
