using System.Threading;
using System.Threading.Tasks;
using MrCMS.Website;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace MrCMS.DbConfiguration;

public class ValidateNHibernateSchema : IExecuteOnStartup
{
    private readonly IGetNHibernateConfiguration _getNHibernateConfiguration;

    public ValidateNHibernateSchema(IGetNHibernateConfiguration getNHibernateConfiguration)
    {
        _getNHibernateConfiguration = getNHibernateConfiguration;
    }
        
    public async Task Execute(CancellationToken cancellationToken)
    {
        var config = _getNHibernateConfiguration.GetConfiguration();
        var validator = new SchemaValidator(config);
        try
        {
            await validator.ValidateAsync(cancellationToken);
        }
        catch (HibernateException)
        {
            var update = new SchemaUpdate(config);
            await update.ExecuteAsync(false, true, cancellationToken);
        }

    }

    public int Order => 0;
}