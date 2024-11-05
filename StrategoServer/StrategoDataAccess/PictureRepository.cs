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
        private readonly Lazy<StrategoEntities> _context;

        public PictureRepository(Lazy<StrategoEntities> context)
        {
            _context = context;
        }

        public Result<Pictures> GetPictureById(int pictureId)
        {
            try
            {
                var picture = _context.Value.Pictures.FirstOrDefault(p => p.IdPicture == pictureId);

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
