## Auteur
**Abdelouhab Belkhiri**

# Routing Web service
> The Routing server will retrieve the geo data to compute itineraries for the user to get a bike,
> drive to the station where to drop the bike, and reach its destination
> There are mulitple **endpoints** to access this service :
> Expose **REST** and **SOAP** communication
> It use Web Proxy service intermediate for JcDecaux API
> **it's a self hosted service**

# RUN it !
    - Make sure all ports are free (i.e : 8722, 8734, 8736)
    - Make sure to run Hosting.exe with **administrator priviledges**
    - HostingWPS.exe is in folder **WebProxyService/Hosting/bin/Debug/HostingWPS.exe**

# Features!
  - Calling **JcDecauxApi** (*RestApiCaller.cs*) 
  - Generic cache system with JCDECAUX items
  - Verify if station exist in Cache else perform Api call to Jcdecaux
  - **Cache with time system to clear cache**
  - Use **JcDecauxItem** class to store station infos (*nameStation / bikeAvailable / standsAvailable*)
  - Debug Mode --> put boolean **DEBUG** to true if you want to see logs
  - Debug Mode is inspired by **[Subject Debug](https://stackoverflow.com/questions/637117/how-to-get-the-tsql-query-from-linq-datacontext-submitchanges/637151#637151)**

# Folders structure
   - In **apiHandler** folder : to call REST JcDecaux API (RespApiCaller.cs)
   - In **buisinessObjects** folder : buisiness object : *JcDeacauxItem*
   - In **caching** folder : **IProxyCache** and
    -- **ProxyCache.cs** : all methos for cache purposes (*time / period...*)
   - In **root** : **IWebProxyService** exposed with **rest**
    -- **WebProxyService.cs** : all buisiness logic with methods (*getInfosStation / getAllStations...*)
    - In **Debug** folder : class that allows to print logs in wcf services
            



