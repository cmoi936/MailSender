# 📧 MailSender

[![Build and Push Docker Image](https://github.com/cmoi936/MailSender/actions/workflows/docker-publish.yml/badge.svg)](https://github.com/cmoi936/MailSender/actions/workflows/docker-publish.yml)
[![Docker Image](https://img.shields.io/badge/docker-ghcr.io%2Fcmoi936%2Fmailsender-blue)](https://github.com/cmoi936/MailSender/pkgs/container/mailsender)
[![Docker MCP Image](https://img.shields.io/badge/docker-ghcr.io%2Fcmoi936%2Fmailsender--mcp-green)](https://github.com/cmoi936/MailSender/pkgs/container/mailsender-mcp)

A simple and efficient solution for sending emails via SMTP, available as both a REST API and an MCP (Model Context Protocol) server. Built with .NET 8 and fully containerized with Docker.

## 🚀 Quick Start

### REST API - Using Pre-built Docker Image (Recommended)

```bash
# Download and run in one command
docker run -d \
  --name mailsender-api \
  -p 5000:8080 \
  -e SMTP__USERNAME="your-email@gmail.com" \
  -e SMTP__PASSWORD="your-app-password" \
  -e SMTP__FROMEMAIL="your-email@gmail.com" \
  ghcr.io/cmoi936/mailsender:latest
```

### MCP Server - Using Pre-built Docker Image

```bash
# Download and run the MCP server
docker run -i \
  -e SMTP__USERNAME="your-email@gmail.com" \
  -e SMTP__PASSWORD="your-app-password" \
  -e SMTP__FROMEMAIL="your-email@gmail.com" \
  ghcr.io/cmoi936/mailsender-mcp:latest
```

The MCP server uses stdio transport for communication with AI assistants and tools.

### Option 2: Using Docker Compose

1. **Clone the repository**:
```bash
git clone https://github.com/cmoi936/MailSender.git
cd MailSender
```

2. **Configure environment variables**:
```bash
cp src/.env.example src/.env
# Edit .env with your real SMTP values
```

3. **Deploy**:
```bash
# Windows (from repository root)
cd src
.\deploy.ps1

# Linux/macOS (from repository root)
cd src
./deploy.sh

# Or manually (from repository root)
docker-compose -f src/docker-compose.production.yml up -d
```

## 📋 Features

- ✅ **REST API** for sending emails
- ✅ **MCP Server** for AI integration (Model Context Protocol)
- ✅ **SMTP Support** (Gmail, Outlook, etc.)
- ✅ **Dockerized** with multi-architecture support (AMD64, ARM64)
- ✅ **Health checks** built-in
- ✅ **CC/BCC Support** (multiple recipients)
- ✅ **HTML and plain text** messages
- ✅ **Logging** configured
- ✅ **Secure** (non-root user in container)
- ✅ **CI/CD** automated with GitHub Actions
- ✅ **Swagger/OpenAPI** documentation
- ✅ **Shared Core library** for code reuse

## 🏗️ Available Docker Images

### REST API Image

| Tag | Description | Platform |
|-----|-------------|----------|
| `latest` | Latest stable version | `linux/amd64`, `linux/arm64` |
| `v1.0.0` | Tagged version | `linux/amd64`, `linux/arm64` |
| `master` | Master branch | `linux/amd64`, `linux/arm64` |

Available at: **`ghcr.io/cmoi936/mailsender`**

### MCP Server Image

| Tag | Description | Platform |
|-----|-------------|----------|
| `latest` | Latest stable version | `linux/amd64`, `linux/arm64` |
| `v1.0.0` | Tagged version | `linux/amd64`, `linux/arm64` |
| `master` | Master branch | `linux/amd64`, `linux/arm64` |

Available at: **`ghcr.io/cmoi936/mailsender-mcp`**

## 🔧 Configuration

### Required Environment Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `SMTP__USERNAME` | Your SMTP email | `user@gmail.com` |
| `SMTP__PASSWORD` | Application password | `abcd efgh ijkl mnop` |
| `SMTP__FROMEMAIL` | Sender email | `user@gmail.com` |

### Optional Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `SMTP__HOST` | `smtp.gmail.com` | SMTP server |
| `SMTP__PORT` | `587` | SMTP port |
| `SMTP__FROMNAME` | `MailSender API` | Sender name |
| `SMTP__USESSL` | `true` | Use SSL/TLS |
| `SMTP__TIMEOUTMS` | `30000` | Timeout in ms |

### Gmail Configuration

1. **Enable 2-factor authentication** on your Google account
2. **Generate an app password**:
   - Google Account → Security → 2-Step Verification
   - App passwords → Create new password for "MailSender"
   - Copy the generated password (16 characters)
   - Use this password in `SMTP__PASSWORD`

## 📡 API Usage

### Health Check
```bash
GET http://localhost:5000/api/health
```

### Send an Email
```bash
POST http://localhost:5000/api/email/send
Content-Type: application/json

{
  "to": "recipient@example.com",
  "cc": "copy@example.com",        // optional (multiple emails separated by ;)
  "bcc": "blind-copy@example.com", // optional (multiple emails separated by ;)
  "subject": "Hello from MailSender!",
  "message": "This is a test message."
}
```

### Example with curl
```bash
curl -X POST http://localhost:5000/api/email/send \
  -H "Content-Type: application/json" \
  -d '{
    "to": "test@example.com",
    "subject": "Test from Docker",
    "message": "Hello World from MailSender API!"
  }'
```

## 🤖 MCP Server Usage

The MCP (Model Context Protocol) server allows AI assistants to send emails. It exposes a `SendEmail` tool that can be used by any MCP-compatible client.

### Available Tools

#### SendEmail

Send an email via SMTP.

**Parameters:**
- `to` (required): Recipient email address
- `subject` (required): Email subject
- `message` (required): Email message body (can be HTML)
- `cc` (optional): CC recipients, separated by semicolons
- `bcc` (optional): BCC recipients, separated by semicolons

### Integration with AI Assistants

To integrate the MCP server with your AI assistant, configure it to use the Docker image:

```json
{
  "mcpServers": {
    "mailsender": {
      "command": "docker",
      "args": [
        "run", "-i", "--rm",
        "-e", "SMTP__USERNAME=your-email@gmail.com",
        "-e", "SMTP__PASSWORD=your-app-password",
        "-e", "SMTP__FROMEMAIL=your-email@gmail.com",
        "ghcr.io/cmoi936/mailsender-mcp:latest"
      ]
    }
  }
}
```

## 🛠️ Local Development

### Prerequisites
- .NET 8.0 SDK
- Docker (optional, for containerized development)

### Build with .NET CLI
```bash
# Clone the repository
git clone https://github.com/cmoi936/MailSender.git
cd MailSender

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the API
cd src
dotnet run

# The API will be available at https://localhost:7xxx
# Swagger UI at https://localhost:7xxx/swagger
```

### Build with Docker
```bash
# Build local image
cd src
docker build -t mailsender-local .

# Run local container
docker run -p 5000:8080 mailsender-local
```

### Running Tests
```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal
```

## 📁 Project Structure

```
MailSender/
├── src/                           # Main application source
│   ├── MailSender.Core/          # Shared core library
│   │   ├── Models/               # Shared data models
│   │   │   ├── EmailRequest.cs   # Email request model
│   │   │   ├── EmailResponse.cs  # Email response model
│   │   │   └── SmtpSettings.cs   # SMTP configuration
│   │   └── Services/             # Shared services
│   │       ├── IEmailService.cs  # Email service interface
│   │       └── SmtpEmailService.cs # SMTP implementation
│   ├── MailSender.Mcp/           # MCP Server project
│   │   ├── Tools/                # MCP tools
│   │   │   └── EmailTools.cs     # Email sending tool
│   │   ├── Program.cs            # MCP server entry point
│   │   └── Dockerfile            # MCP Docker configuration
│   ├── Controllers/              # API controllers
│   │   ├── EmailController.cs   # Email sending endpoint
│   │   └── HealthController.cs  # Health check endpoint
│   ├── Services/                 # API-specific services
│   │   └── EmailService.cs       # API email service wrapper
│   ├── Program.cs                # API entry point
│   ├── Dockerfile                # API Docker configuration
│   ├── docker-compose.yml        # Development compose
│   ├── docker-compose.production.yml  # Production compose
│   ├── deploy.ps1                # Windows deployment script
│   └── deploy.sh                 # Linux/macOS deployment script
├── tests/                         # Test projects
│   └── MailSender.Tests/         # Unit and integration tests
├── .github/                       # GitHub configuration
│   └── workflows/                # CI/CD workflows
│       └── docker-publish.yml    # Docker build and publish
├── MailSender.sln                # Visual Studio solution
└── README.md                     # This file
```

## 🚢 Deployment

### Automatic Deployment

The project uses **GitHub Actions** to automate:
- ✅ Multi-architecture builds (AMD64, ARM64)
- ✅ Automated tests
- ✅ Publishing to GitHub Container Registry
- ✅ Cryptographic signing with Cosign
- ✅ Automatic versioning with Git tags

### Creating a New Release
```bash
# Create and push a tag
git tag v1.0.0
git push origin v1.0.0

# The image will be automatically built and published to ghcr.io
```

### Manual Server Deployment
```bash
# Pull the latest version
docker pull ghcr.io/cmoi936/mailsender:latest

# Use the deployment script (from repository root)
cd src
.\deploy.ps1 latest

# Or use Docker Compose directly (from repository root)
docker-compose -f src/docker-compose.production.yml up -d
```

## 📊 Monitoring and Debugging

```bash
# Real-time logs
docker logs -f mailsender-api

# Performance statistics
docker stats mailsender-api

# Health check
curl http://localhost:5000/api/health

# Access Swagger UI (in development)
# http://localhost:5000/swagger
```

## 🚨 Security

- ✅ Non-root user in Docker container
- ✅ Environment variables for secrets
- ✅ HTTPS support (configurable)
- ✅ Cryptographically signed Docker images
- ✅ Automated vulnerability scanning
- ⚠️ **Important**: Never use your main Gmail password
- ⚠️ **Important**: Always use app passwords

## 📖 Additional Documentation

- 📋 **Docker Deployment Guide**: [src/DOCKER_DEPLOYMENT.md](src/DOCKER_DEPLOYMENT.md)
- 📋 **Detailed API Documentation**: [src/README.md](src/README.md)
- 🐳 **Docker Images**: [GitHub Container Registry](https://github.com/cmoi936/MailSender/pkgs/container/mailsender)
- 🔧 **CI/CD Pipeline**: [GitHub Actions](https://github.com/cmoi936/MailSender/actions)

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the project
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📞 Support

- 📖 **Documentation**: See documentation files in the repository
- 🐛 **Issues**: [GitHub Issues](https://github.com/cmoi936/MailSender/issues)
- 💬 **Discussions**: [GitHub Discussions](https://github.com/cmoi936/MailSender/discussions)

## 📄 License

This project is licensed under the MIT License. See the `LICENSE` file for details.

---

**Developed with ❤️ using .NET 8**
