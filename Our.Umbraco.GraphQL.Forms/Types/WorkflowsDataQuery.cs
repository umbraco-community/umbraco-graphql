using Our.Umbraco.GraphQL.Attributes;
using System;
using System.Collections.Generic;
using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Interfaces;

namespace Our.Umbraco.GraphQL.Forms.Types
{
    public class WorkflowsDataQuery
    {
        public IEnumerable<IWorkflow> All([Inject] IWorkflowStorage workflowStorage) => workflowStorage.GetAllWorkFlows();

        public IWorkflow ById([Inject] IWorkflowStorage workflowStorage, Guid id) => workflowStorage.GetWorkflow(id);
    }
}
