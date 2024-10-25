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

        public Task<Result<string>> CreateAccountAsync(string email, string password, string playername)
        {
            var result = _accountRepository.Value.CreateAccountAsync(email, password, playername);
            return result;
        }

        public Task<Result<int>> LogInAccountAsync(string email, string password)
        {
            var result = _accountRepository.Value.ValidateCredentialsAsync(email, password);
            return result;
        }

        public async Task<Result<PlayerDTO>> GetLogInAccountAsync(int accountId)
        {
            var result = await _playerRepository.Value.GetPlayerByAccountIdAsync(accountId);

            if (!result.IsSuccess)
            {
                return Result<PlayerDTO>.Failure(result.Error);
            }

            var player = result.Value;

            var pictureTask = _picturesRepository.Value.GetPictureByIdAsync((int)player.PictureId);
            var labelTask = _labelRepository.Value.GetLabelByIdAsync(player.IdLabel);

            await Task.WhenAll(pictureTask, labelTask);

            var pictureResult = await pictureTask;
            var labelResult = await labelTask;

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
