using StrategoDataAccess;
using StrategoServices.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace StrategoServices.Logic
{
    public class AccountManager
    {
        private readonly Lazy<AccountRepository> _accountRepository;
        private readonly Lazy<PlayerRepository> _playerRepository;
        private readonly Lazy<PictureRepository> _picturesRepository;
        private readonly Lazy<LabelRepository> _labelRepository;

        public AccountManager(Lazy<AccountRepository> accountRepository, Lazy<PlayerRepository> playerRepository, 
            Lazy<PictureRepository> pictureRepository, Lazy<LabelRepository> labelRepository)
        {
            _accountRepository = accountRepository;
            _playerRepository = playerRepository;
            _picturesRepository = pictureRepository;
            _labelRepository = labelRepository;
        }

        public Result<string> CreateAccount(string email, string password, string playername)
        {
            var result = _accountRepository.Value.CreateAccount(email, password, playername);
            return result;
        }

        public Result<int> LogInAccount(string email, string password)
        {
            var result = _accountRepository.Value.ValidateCredentials(email, password);
            return result;
        }

        public Result<PlayerDTO> GetLogInAccount(int accountId)
        {
            var result = _playerRepository.Value.GetPlayerByAccountId(accountId);

            if (!result.IsSuccess)
            {
                return Result<PlayerDTO>.Failure(result.Error);
            }

            var player = result.Value;

            var pictureResult = _picturesRepository.Value.GetPictureById((int)player.PictureId);
            var labelResult = _labelRepository.Value.GetLabelById(player.IdLabel);

            if (!pictureResult.IsSuccess)
            {
                return Result<PlayerDTO>.Failure(pictureResult.Error);
            }

            if (!labelResult.IsSuccess)
            {
                return Result<PlayerDTO>.Failure(labelResult.Error);
            }

            var picturePath = pictureResult.Value.path ?? "picture1";
            var labelPath = labelResult.Value.Path ?? "label1";

            var playerDTO = new PlayerDTO
            {
                Id = player.Id,
                Name = player.Name ?? string.Empty,
                PicturePath = picturePath,
                LabelPath = labelPath,
                AccountId = (int)player.AccountId
            };

            return Result<PlayerDTO>.Success(playerDTO);
        }

    }
}
