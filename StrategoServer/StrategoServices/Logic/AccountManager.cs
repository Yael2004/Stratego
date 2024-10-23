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
        private readonly AccountRepository _accountRepository;
        private readonly PlayerRepository _playerRepository;
        private readonly PictureRepository _picturesRepository;
        private readonly LabelRepository _labelRepository;

        public AccountManager(AccountRepository accountRepository, PlayerRepository playerRepository, PictureRepository pictureRepository, LabelRepository labelRepository)
        {
            _accountRepository = accountRepository;
            _playerRepository = playerRepository;
            _picturesRepository = pictureRepository;
            _labelRepository = labelRepository;
        }

        public Task<Result<string>> CreateAccountAsync(string email, string password, string playername)
        {
            var result = _accountRepository.CreateAccountAsync(email, password, playername);
            return result;
        }

        public Task<Result<int>> LogInAccountAsync(string email, string password)
        {
            var result = _accountRepository.ValidateCredentialsAsync(email, password);
            return result;
        }

        public async Task<Result<PlayerDTO>> GetLogInAccountAsync(int accountId)
        {
            var result = await _playerRepository.GetPlayerByAccountIdAsync(accountId);

            if (!result.IsSuccess)
            {
                return Result<PlayerDTO>.Failure(result.Error);
            }

            var player = result.Value;

            var pictureTask = _picturesRepository.GetPictureByIdAsync((int)player.PictureId);
            var labelTask = _labelRepository.GetLabelByIdAsync(player.IdLabel);

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
