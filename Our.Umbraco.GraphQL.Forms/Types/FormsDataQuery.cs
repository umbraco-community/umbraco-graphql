using Our.Umbraco.GraphQL.Attributes;
using System;
using System.Collections.Generic;
using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Models;

namespace Our.Umbraco.GraphQL.Forms.Types
{
    public class FormsDataQuery
    {
        public IEnumerable<Form> All([Inject] IFormStorage formStorage) => formStorage.GetAllForms();

        public Form ById([Inject] IFormStorage formStorage, string id) => Guid.TryParse(id, out var formId) ? formStorage.GetForm(formId) : null;

        public Form ByName([Inject] IFormStorage formStorage, string name) => formStorage.GetForm(name);
    }
}
