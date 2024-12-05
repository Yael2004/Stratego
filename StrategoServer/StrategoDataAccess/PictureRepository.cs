using log4net;
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
        private static readonly ILog log = LogManager.GetLogger(typeof(PictureRepository));

        public virtual Result<Pictures> GetPictureById(int pictureId)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var picture = context.Pictures.FirstOrDefault(p => p.IdPicture == pictureId);

                    if (picture == null)
                    {
                        return Result<Pictures>.Failure("Picture not found");
                    }

                    return Result<Pictures>.Success(picture);
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error("Database error", sqlEx);
                return Result<Pictures>.Failure($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error("Unexpected error", ex);
                return Result<Pictures>.Failure($"Unexpected error: {ex.Message}");
            }
        }
    }
}
