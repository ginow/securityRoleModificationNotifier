using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TestPluginAssembly
{
    public class UserSecurityRoleChanged : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            tracingService.Trace("In Execute method of UserSecurityRoleChanged plugin");

            // Obtain the organization service reference which you will need for  
            // web service calls.  
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                EntityReference targetEntity = null;
                string relationshipName = string.Empty;
                EntityReferenceCollection relatedEntities = null;
                EntityReference relatedEntity = null;

                if (context.MessageName == "Associate" || context.MessageName == "Disassociate") 
                {
                    tracingService.Trace("MessageName is: "+context.MessageName);
                    // Get the "Relationship" Key from context
                    if (context.InputParameters.Contains("Relationship"))
                    {
                        relationshipName = context.InputParameters["Relationship"].ToString();
                        tracingService.Trace("relationshipName: "+relationshipName);
                    }
                    // Check the "Relationship Name" with your intended one
                    if (relationshipName.Equals("systemuserroles_association"))
                    {
                        tracingService.Trace("relationshipName is systemuserroles_association");
                    }
                    if (relationshipName.Equals("systemuserroles_association."))
                    {
                        tracingService.Trace("relationshipName is systemuserroles_association.");
                    }
                    if (relationshipName.Contains("systemuserroles_association"))
                    {
                        tracingService.Trace("relationshipName contains systemuserroles_association");
                    }
                    // Get Entity 1 reference from "Target" Key from context
                    if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
                    {
                        targetEntity = (EntityReference)context.InputParameters["Target"];
                        tracingService.Trace("target is entityreference, name: "+targetEntity.Name+" logicalname: "+targetEntity.LogicalName+" id: "+targetEntity.Id +" keyattributes: "+targetEntity.KeyAttributes+"-----"+targetEntity.KeyAttributes.Count+"------"+targetEntity.KeyAttributes.Values);
                    }

                    Entity flowemail = new Entity("new_flowemail");
                    flowemail["new_name"] = "createdfromplugin";
                    
                    Entity user=service.Retrieve("systemuser", targetEntity.Id, new ColumnSet("fullname"));
                    flowemail.Attributes["new_emailbody"] = "User whose security role modified: "+ user.GetAttributeValue<string>("fullname")+"\n";

                    // Get Entity 2 reference from " RelatedEntities" Key from context
                    if (context.InputParameters.Contains("RelatedEntities") && context.InputParameters["RelatedEntities"] is EntityReferenceCollection)
                    {
                        relatedEntities = context.InputParameters["RelatedEntities"] as EntityReferenceCollection;
                        relatedEntity = relatedEntities[0];
                        tracingService.Trace("contains related entities: "+relatedEntity.LogicalName+" count: "+relatedEntities.Count);
                        Entity role = null;
                        if (context.MessageName=="Associate")
                        {
                            flowemail.Attributes["new_emailbody"] += "Roles added: \n";
                        }
                        else
                        {
                            flowemail.Attributes["new_emailbody"] += "Roles removed: \n";
                        }                        
                        foreach (EntityReference item in relatedEntities)
                        {
                            tracingService.Trace("Name: "+ item.Name + " Id: "+ item.Id + " Keyattributes.Values: "+ item.KeyAttributes.Values + " Keyattributes.Keys: "+item.KeyAttributes.Keys+" Keyattributes.Count: "+ item.KeyAttributes.Count + " \n");
                            role = service.Retrieve("role", item.Id, new ColumnSet("name"));
                            flowemail.Attributes["new_emailbody"] += role.GetAttributeValue<string>("name")+ " \n";
                        }
                    }
                    service.Create(flowemail);
                }
            }

            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new InvalidPluginExecutionException("An error occurred in UserSecurityRoleChanged.", ex);
            }

            catch (Exception ex)
            {
                tracingService.Trace("UserSecurityRoleChanged: {0}", ex.ToString());
                throw;
            }
        }
    }
}
