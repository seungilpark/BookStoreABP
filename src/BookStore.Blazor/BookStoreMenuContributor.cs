﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BookStore.Localization;
using Volo.Abp.Account.Localization;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Users;
using BookStore.Permissions;

namespace BookStore.Blazor
{
    public class BookStoreMenuContributor : IMenuContributor
    {
        private readonly IConfiguration _configuration;

        public BookStoreMenuContributor(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task ConfigureMenuAsync(MenuConfigurationContext context)
        {
            if (context.Menu.Name == StandardMenus.Main)
            {
                await ConfigureMainMenuAsync(context);
            }
            else if (context.Menu.Name == StandardMenus.User)
            {
                await ConfigureUserMenuAsync(context);
            }
        }

        private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<BookStoreResource>();

            context.Menu.Items.Insert(
                0,
                new ApplicationMenuItem(
                    "BookStore.Home",
                    l["Menu:Home"],
                    "/",
                    icon: "fas fa-home"
                )
            );


            //context.Menu.AddItem(
            //    new ApplicationMenuItem(
            //        "BooksStore",
            //        l["Menu:BookStore"],
            //        icon: "fa fa-book"
            //    ).AddItem(
            //        new ApplicationMenuItem(
            //            "BooksStore.Books",
            //            l["Menu:Books"],
            //            url: "/books"
            //        )
            //    )
            //);
            var bookStoreMenu = new ApplicationMenuItem(
                "BooksStore",
                l["Menu:BookStore"],
                icon: "fa fa-book"
            );

            context.Menu.AddItem(bookStoreMenu);

            //CHECK the PERMISSION
            if (await context.IsGrantedAsync(BookStorePermissions.Books.Default))
            {
                bookStoreMenu.AddItem(new ApplicationMenuItem(
                    "BooksStore.Books",
                    l["Menu:Books"],
                    url: "/books"
                ));
            }

            if (await context.IsGrantedAsync(BookStorePermissions.Authors.Default))
            {
                bookStoreMenu.AddItem(new ApplicationMenuItem(
                    "BooksStore.Authors",
                    l["Menu:Authors"],
                    url: "/authors"
                ));
            }


        }

        private Task ConfigureUserMenuAsync(MenuConfigurationContext context)
        {
            var accountStringLocalizer = context.GetLocalizer<AccountResource>();
            var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();

            var identityServerUrl = _configuration["AuthServer:Authority"] ?? "";

            if (currentUser.IsAuthenticated)
            {
                context.Menu.AddItem(new ApplicationMenuItem(
                    "Account.Manage",
                    accountStringLocalizer["ManageYourProfile"],
                    $"{identityServerUrl.EnsureEndsWith('/')}Account/Manage",
                    icon: "fa fa-cog",
                    order: 1000,
                    null,
                    "_blank"));
            }

            return Task.CompletedTask;
        }
    }
}
