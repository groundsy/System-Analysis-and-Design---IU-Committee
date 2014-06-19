/************************************************
 * Team: Team Banana
 * Programmers: Justin Ashdown, Eric Grounds, Joel Haubold, Jared Short, Dung Truong
 * 
 * File: App_Start/RouteConfig.cs
 * Created by: MVC Framework
 * Created date: 11-17-12
 * Primary Programmer:
 * File description: Contains route control information for routing urls to controllers
 * 
 * Change Log:
 * Date  programmer    change
 * 11-17-12 jh	 Added "TwoParams route"
 * 
*************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TeamBananaPhase4
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "SixParams",
                url: "{controller}/{action}/{primaryKey1}/{primaryKey2}/{primaryKey3}/{primaryKey4}/{primaryKey5}/{primaryKey6}");

            routes.MapRoute(
                name: "FiveParams",
                url: "{controller}/{action}/{primaryKey1}/{primaryKey2}/{primaryKey3}/{primaryKey4}/{primaryKey5}");

            routes.MapRoute(
                name: "FourParams",
                url: "{controller}/{action}/{primaryKey1}/{primaryKey2}/{primaryKey3}/{primaryKey4}");

            routes.MapRoute(
                name: "ThreeParams",
                url: "{controller}/{action}/{primaryKey1}/{primaryKey2}/{primaryKey3}");

            routes.MapRoute(
                name: "TwoParams",
                url: "{controller}/{action}/{primaryKey1}/{primaryKey2}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{primaryKey1}",
                defaults: new { controller = "Divisions", action = "Index", primaryKey1 = UrlParameter.Optional }

            );
        }
    }
}