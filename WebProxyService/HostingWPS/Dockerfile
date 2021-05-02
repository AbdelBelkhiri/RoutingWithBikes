# Start with Windows Server Core image
FROM mcr.microsoft.com/windows/servercore:ltsc2019

RUN powershell -Command Add-WindowsFeature NET-WCF-HTTP-Activation45

# Creates a directory for the Host
WORKDIR app

# Copy the WCF host into the container.
COPY bin .

CMD ["Debug/HostingWPS.exe"]