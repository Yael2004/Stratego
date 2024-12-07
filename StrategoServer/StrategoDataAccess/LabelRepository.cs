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
    public class LabelRepository
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LabelRepository));

        public virtual Result<Label> GetLabelById(int labelId)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var label = context.Label.FirstOrDefault(l => l.IdLabel == labelId);

                    if (label == null)
                    {
                        return Result<Label>.Failure("Label not found");
                    }

                    return Result<Label>.Success(label);
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<Label>.DataBaseError($"{Messages.DataBaseError}: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<Label>.Failure($"{Messages.UnexpectedError} : {ex.Message}");
            }
        }
    }
}
