﻿//-----------------------------------------------------------------------------
// <copyright file="BulkOperationEdmModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNet.OData.Builder;
using Microsoft.OData.Edm;
using Microsoft.Test.E2E.AspNet.OData.Common.Execution;

namespace Microsoft.Test.E2E.AspNet.OData.BulkOperation
{
    internal class BulkOperationEdmModel
    {
        public static IEdmModel GetExplicitModel(WebRouteConfiguration configuration)
        {
            ODataModelBuilder builder = new ODataModelBuilder();
            var employee = builder.EntityType<Employee>();
            employee.HasKey(c => c.ID);
            employee.Property(c => c.Name);
            employee.CollectionProperty<Skill>(c => c.SkillSet);
            employee.EnumProperty<Gender>(c => c.Gender);
            employee.EnumProperty<AccessLevel>(c => c.AccessLevel);
            
            employee.CollectionProperty<Friend>(c => c.Friends);
            employee.CollectionProperty<NewFriend>(c => c.NewFriends);
            employee.CollectionProperty<UnTypedFriend>(c => c.UnTypedFriends);


            var skill = builder.EnumType<Skill>();
            skill.Member(Skill.CSharp);
            skill.Member(Skill.Sql);
            skill.Member(Skill.Web);

            var gender = builder.EnumType<Gender>();
            gender.Member(Gender.Female);
            gender.Member(Gender.Male);

            var accessLevel = builder.EnumType<AccessLevel>();
            accessLevel.Member(AccessLevel.Execute);
            accessLevel.Member(AccessLevel.Read);
            accessLevel.Member(AccessLevel.Write);

            var sport = builder.EnumType<Sport>();
            sport.Member(Sport.Basketball);
            sport.Member(Sport.Pingpong);

            AddBoundActionsAndFunctions(employee);
            AddUnboundActionsAndFunctions(builder);

            EntitySetConfiguration<Employee> employees = builder.EntitySet<Employee>("Employees");
            builder.Namespace = typeof(Employee).Namespace;
            return builder.GetEdmModel();
        }

        public static IEdmModel GetConventionModel(WebRouteConfiguration configuration)
        {
            ODataConventionModelBuilder builder = configuration.CreateConventionModelBuilder();
            EntitySetConfiguration<Employee> employees = builder.EntitySet<Employee>("Employees");
            EntityTypeConfiguration<Employee> employee = employees.EntityType;

            EntitySetConfiguration<Friend> friends = builder.EntitySet<Friend>("Friends");
            EntitySetConfiguration<Order> orders = builder.EntitySet<Order>("Orders");
            EntitySetConfiguration<NewFriend> fnewriends = builder.EntitySet<NewFriend>("NewFriends");
            EntitySetConfiguration<UnTypedFriend> funtypenewriends = builder.EntitySet<UnTypedFriend>("UnTypedFriends");
            EntitySetConfiguration<UnTypedAddress> addresses = builder.EntitySet<UnTypedAddress>("Address");

            EntitySetConfiguration<UnTypedEmployee> unemployees = builder.EntitySet<UnTypedEmployee>("UnTypedEmployees");
            EntityTypeConfiguration<UnTypedEmployee> unemployee = unemployees.EntityType;

            EntitySetConfiguration<Company> companies = builder.EntitySet<Company>("Companies");
            EntitySetConfiguration<NewOrder> overdueorders = builder.EntitySet<NewOrder>("OverdueOrders");
            EntitySetConfiguration<MyNewOrder> myoverdueorders = builder.EntitySet<MyNewOrder>("MyOverdueOrders");
            EntitySetConfiguration<NewOrder> myNewOrders = builder.EntitySet<NewOrder>("MyNewOrders");

            // maybe following lines are not required once bug #1587 is fixed.
            // 1587: It's better to support automatically adding actions and functions in ODataConventionModelBuilder.
            AddBoundActionsAndFunctions(employee);
            AddUnboundActionsAndFunctions(builder);

            builder.Namespace = typeof(Employee).Namespace;
            builder.MaxDataServiceVersion = EdmConstants.EdmVersion401;
            builder.DataServiceVersion = EdmConstants.EdmVersion401;

            var edmModel = builder.GetEdmModel();
            return edmModel;
        }

        private static void AddBoundActionsAndFunctions(EntityTypeConfiguration<Employee> employee)
        {
            var actionConfiguration = employee.Action("AddSkill");
            actionConfiguration.Parameter<Skill>("skill");
            actionConfiguration.ReturnsCollection<Skill>();

            var functionConfiguration = employee.Function("GetAccessLevel");
            functionConfiguration.Returns<AccessLevel>();
        }

        private static void AddUnboundActionsAndFunctions(ODataModelBuilder odataModelBuilder)
        {
            var actionConfiguration = odataModelBuilder.Action("SetAccessLevel");
            actionConfiguration.Parameter<int>("ID");
            actionConfiguration.Parameter<AccessLevel>("accessLevel");
            actionConfiguration.Returns<AccessLevel>();

            var functionConfiguration = odataModelBuilder.Function("HasAccessLevel");
            functionConfiguration.Parameter<int>("ID");
            functionConfiguration.Parameter<AccessLevel>("AccessLevel");
            functionConfiguration.Returns<bool>();
        }
    }
}