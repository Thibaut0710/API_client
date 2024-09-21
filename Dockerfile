FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

RUN apt-get update && apt-get install -y mariadb-server

ENV MYSQL_ROOT_PASSWORD=zDH-.42l3lSRIT1p
ENV MYSQL_DATABASE=clients_db
ENV MYSQL_USER=client_user
ENV MYSQL_PASSWORD=azerty

WORKDIR /app/out

COPY init-mariadb.sh /usr/local/bin/init-mariadb.sh
RUN chmod +x /usr/local/bin/init-mariadb.sh
RUN dotnet dev-certs https -ep /https/aspnetapp.pfx -p "YourPassword" && \
    dotnet dev-certs https --trust

EXPOSE 3306 7296

CMD ["init-mariadb.sh"]
