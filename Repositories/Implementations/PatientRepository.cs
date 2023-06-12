using System.Globalization;
using Document_Extractor.Data;
using Document_Extractor.Models.DB;
using Document_Extractor.Models.Shared;
using Document_Extractor.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Document_Extractor.Repositories.Implementation;


public class PatientRepository : IPatientRepository
{
    private readonly AppDbContext _context;
    public PatientRepository(
        AppDbContext context
        )
    {
        _context = context;
    }

    public async Task<Patient> Create(Patient item)
    {
        _context.Entry(item).State = EntityState.Added;
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task<IEnumerable<Patient>> CreateMany(IEnumerable<Patient> items)
    {
        _context.Patients.AddRange(items);
        await _context.SaveChangesAsync();
        return items;
    }

    public async Task<bool> Delete(long itemId)
    {
        var Patient = await GetOne(itemId);
        if (Patient == null) throw new Exception($"Error fetching Patient with id ={itemId}");

        _context.Entry(Patient).State = EntityState.Deleted;
        await _context.SaveChangesAsync();

        return true;

    }

    public async Task<IEnumerable<Patient>> GetAll()
    {
        var result = await _context.Patients.ToListAsync();
        return result;
    }

    public async Task<Patient> GetOne(long itemId)
    {
        var result = await _context.Patients.FirstOrDefaultAsync(x => x.PatientId == itemId);
        return result;
    }

    public async Task<bool> Update(Patient item, long itemId)
    {
        var Patient = await GetOne(itemId);
        if (Patient == null) throw new Exception($"Error fetching Patient with id ={itemId}");

        _context.Entry(Patient).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<AppResult<bool>> DoesUploadExist(Dictionary<string, string> props, Dictionary<string, string> propsAndFormat)
    {
        var result = new AppResult<bool>();

        try
        {
            var query = $"select * from dbo.Patients where ";
            var propKeys = props.Keys.ToList();
            var formatPropKeys = propsAndFormat.Keys.ToList();

            var datetimeProps = props.Where(x => formatPropKeys.Contains(x.Key)).ToList();
            var notDatetimeProps = props.Where(x => !formatPropKeys.Contains(x.Key)).ToList();

            var queryFilter = string.Empty;
            foreach (var itemProp in notDatetimeProps)
            {
                var propKey = itemProp.Key;
                var propValue = itemProp.Value;

                queryFilter += string.IsNullOrEmpty(queryFilter) ? $" {propKey} = '{propValue}'" : $" and {propKey} = '{propValue}'";
            }

            query += queryFilter;

            var uploadedDocs = await _context.Patients.FromSqlRaw(query).ToListAsync();
            if (uploadedDocs.Any())
            {
                var resultDocs = new List<Patient>();
                foreach (var item in datetimeProps)
                {
                    var dKey = item.Key;
                    var dValue = item.Value;

                    var format = propsAndFormat[dKey];
                    var d = DateTime.ParseExact(dValue, format, CultureInfo.InvariantCulture);

                    if (dKey == SharedConstants.DateTime)
                    {
                        var r = uploadedDocs.Where(x => x.DateTime.ToString(format) == d.ToString(format)).ToList();
                        resultDocs.AddRange(r);
                    }
                    else
                    {
                        var r = uploadedDocs.Where(x => x.PatientDOB.ToString(format) == d.ToString(format)).ToList();
                        resultDocs.AddRange(r);
                    }

                }

                bool isUploaded = resultDocs.Any();
                result.Data.Add(isUploaded);

            }
            else
            {
                result.Data.Add(false);
            }

            result.Status = true;

        }
        catch (Exception ex)
        {
            result.Status = false;
            result.Message = ex.Message;
        }

        return result;

    }

    public async Task<AppResult<bool>> ConfirmUpload(long patientId, bool status)
    {
        var result = new AppResult<bool>();
        try
        {
            var dbPatient = await GetOne(patientId);
            if (dbPatient == null) throw new Exception($"Error fetching Patient with id = {patientId}");

            dbPatient.UpdatedAt = DateTime.Now;
            dbPatient.IsUploadComfirmed = status;

            var r =  await Update(dbPatient, dbPatient.PatientId);

            result.Status = true;
            result.Data.Add(r);
            result.Message = $"Patient {patientId}, Confirmation was {r}";

        }
        catch (Exception ex)
        {
            result.Status = false;
            result.Message = ex.Message;
        }
        return result;
    }

}