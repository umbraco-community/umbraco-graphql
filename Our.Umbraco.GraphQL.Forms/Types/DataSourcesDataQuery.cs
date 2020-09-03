using Our.Umbraco.GraphQL.Attributes;
using System;
using System.Collections.Generic;
using Umbraco.Forms.Core;
using Umbraco.Forms.Data.Storage;

namespace Our.Umbraco.GraphQL.Forms.Types
{
    public class DataSourcesDataQuery
    {
        public IEnumerable<FormDataSource> All([Inject] IDataSourceStorage dataSourceStorage) => dataSourceStorage.GetAllDataSources();

        public FormDataSource ById([Inject] IDataSourceStorage dataSourceStorage, string id) => Guid.TryParse(id, out var guid) ? dataSourceStorage.GetDataSource(guid) : null;
    }
}
