FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /src
COPY shared/SharedJwt/ shared/SharedJwt/
COPY auth-service/ auth-service/
WORKDIR /src/auth-service
RUN dotnet publish -c Release -o /app/ -v q
RUN apt-get update && apt-get install -y wget \
 && ARCH=$(dpkg --print-architecture) \
 && wget -q -O /app/gg.deb "https://github.com/grft-dev/graftcode-gateway/releases/latest/download/gg_linux_${ARCH}.deb" \
 && dpkg -i /app/gg.deb && rm /app/gg.deb \
 && apt-get clean && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 80
EXPOSE 8989
CMD ["gg", "--modules", "AuthService.dll", "--useContext=1", "--http2Server=1", "--http2Port", "8989", "--corsAllowedOrigins=*"]
