using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController: ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(
            IPlatformRepo repository, 
            IMapper mapper,
            ICommandDataClient commandDataClient)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("--> Getting Platforms...." );
             var platformItems = _repository.GetAllPlatforms();

             return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems)); 
        }

        [HttpGet("{id}", Name = "GetPlatformsById")]
        // [Route("{id:int}")]
        public ActionResult<PlatformReadDto> GetPlatformsById(int id){
            Platform p = _repository.GetPlatformById(id);
            if(p!=null){
            return Ok(_mapper.Map<PlatformReadDto>(p));
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            Platform obj = _mapper.Map<Platform>(platformCreateDto);

            _repository.CreatePlatform(obj);
            _repository.SaveChanges();
            PlatformReadDto objRead = _mapper.Map<PlatformReadDto>(obj);
            try
            {
                await _commandDataClient.SendPlatformToCommand(objRead);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"---> Could not send synchronously: {ex.Message}");
                Console.WriteLine($"---> Could not send synchronously: {ex.InnerException}");
                
            }
            return CreatedAtRoute(nameof(GetPlatformsById), new {Id = objRead.Id}, objRead);
        }
    }
}