using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace StrategoDataAccess
{
    public class PictureRepository
    {
        private readonly StrategoEntities _context;

        public PictureRepository(StrategoEntities context)
        {
            _context = context;
        }

        public async Task<Result<Pictures>> GetPictureByIdAsync(int pictureId)
        {
            try
            {
                var picture = await _context.Pictures.FirstOrDefaultAsync(p => p.IdPicture == pictureId);

                if (picture == null)
                {
                    return Result<Pictures>.Failure("Picture not found");
                }

                return Result<Pictures>.Success(picture);
            }
            catch (SqlException sqlEx)
            {
                return Result<Pictures>.Failure($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<Pictures>.Failure($"Unexpected error: {ex.Message}");
            }
        }
    }
}
