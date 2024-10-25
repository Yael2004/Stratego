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
        private readonly Lazy<StrategoEntities> _context;

        public LabelRepository(Lazy<StrategoEntities> context)
        {
            _context = context;
        }

        public async Task<Result<Label>> GetLabelByIdAsync(int labelId)
        {
            try
            {
                var label = await _context.Value.Label.FirstOrDefaultAsync(l => l.IdLabel == labelId);

                if (label == null)
                {
                    return Result<Label>.Failure("Label not found");
                }

                return Result<Label>.Success(label);
            }
            catch (SqlException sqlEx)
            {
                return Result<Label>.Failure($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<Label>.Failure($"Unexpected error: {ex.Message}");
            }
        }
    }
}
