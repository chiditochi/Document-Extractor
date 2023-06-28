using System.Globalization;
using Document_Extractor.Data;
using Document_Extractor.Models.DB;
using Document_Extractor.Models.Shared;
using Document_Extractor.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Document_Extractor.Repositories.Implementation;


public class PatientTempRepository : IPatientTempRepository
{
    private readonly AppDbContext _context;
    public PatientTempRepository(
        AppDbContext context
        )
    {
        _context = context;
    }

    public async Task<PatientTemp> Create(PatientTemp item)
    {
        _context.Entry(item).State = EntityState.Added;
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task<IEnumerable<PatientTemp>> CreateMany(IEnumerable<PatientTemp> items)
    {
        _context.PatientTemps.AddRange(items);
        await _context.SaveChangesAsync();
        return items;
    }

    public async Task<bool> Delete(long itemId)
    {
        var PatientTemp = await GetOne(itemId);
        if (PatientTemp == null) throw new Exception($"Error fetching PatientTemp with id ={itemId}");

        _context.Entry(PatientTemp).State = EntityState.Deleted;
        await _context.SaveChangesAsync();

        return true;

    }

    public async Task<IEnumerable<PatientTemp>> GetAll()
    {
        var result = await _context.PatientTemps.Include(x => x.Team).OrderByDescending(x => x.CreatedAt).ToListAsync();
        return result;
    }

    public async Task<PatientTemp> GetOne(long itemId)
    {
        var result = await _context.PatientTemps.FirstOrDefaultAsync(x => x.PatientTempId == itemId);
        return result;
    }

    public async Task<bool> Update(PatientTemp item, long itemId)
    {
        var PatientTemp = await GetOne(itemId);
        if (PatientTemp == null) throw new Exception($"Error fetching PatientTemp with id ={itemId}");

        _context.Entry(PatientTemp).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<AppResult<bool>> DoesUploadExist(Dictionary<string, string> props, Dictionary<string, string> propsAndFormat)
    {
        var result = new AppResult<bool>();

        try
        {
            var query = $"select * from dbo.PatientTemps where IsUploadComfirmed = 1 and Status = 1";
            var propKeys = props.Keys.ToList();
            var formatPropKeys = propsAndFormat.Keys.ToList();

            var datetimeProps = props.Where(x => formatPropKeys.Contains(x.Key)).ToList();
            var notDatetimeProps = props.Where(x => !formatPropKeys.Contains(x.Key)).ToList();

            var queryFilter = string.Empty;
            if (notDatetimeProps.Count > 0) query += " and";
            foreach (var itemProp in notDatetimeProps)
            {
                var propKey = itemProp.Key;
                var propValue = itemProp.Value;

                queryFilter += string.IsNullOrEmpty(queryFilter) ? $" {propKey} = '{propValue}'" : $" and {propKey} = '{propValue}'";
            }

            query += queryFilter;

            var uploadedDocs = await _context.PatientTemps.FromSqlRaw(query).ToListAsync();
            if (uploadedDocs.Any())
            {
                var resultDocs = new List<PatientTemp>();
                foreach (var item in datetimeProps)
                {
                    var dKey = item.Key;
                    var dValue = item.Value;

                    var format = propsAndFormat[dKey];
                    if (dValue.IndexOf("-") > -1 && dKey == SharedConstants.DateTime) format = "yyyy-MM-dd HH:mm";
                    if (dValue.IndexOf("-") > -1 && dKey == SharedConstants.PatientDOB) format = "yyyy-MM-dd";

                    //var d = DateTime.ParseExact(dValue, format, CultureInfo.InvariantCulture);
                    var d = DateTime.Parse(dValue);

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

    // public async Task<AppResult<bool>> ConfirmUpload(long patientTempId, bool status)
    // {
    //     var result = new AppResult<bool>();
    //     try
    //     {
    //         var dbPatient = await GetOne(patientTempId);
    //         if (dbPatient == null) throw new Exception($"Error fetching PatientTemp with id = {patientTempId}");

    //         dbPatient.UpdatedAt = DateTime.Now;
    //         dbPatient.IsUploadComfirmed = true;
    //         dbPatient.Status = status;

    //         var r = await Update(dbPatient, dbPatient.PatientTempId);

    //         result.Status = true;
    //         result.Data.Add(r);
    //         result.Message = status ? $"Upload was Confirmed!" : $"Upload was CANCELLED!";

    //     }
    //     catch (Exception ex)
    //     {
    //         result.Status = false;
    //         result.Message = ex.Message;
    //     }
    //     return result;
    // }



}