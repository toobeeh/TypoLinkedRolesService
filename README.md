# Typo linked Roles Service
[![part of Typo ecosystem](https://img.shields.io/badge/Typo%20ecosystem-TypoLinkedRolesService-blue?style=flat&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAIAAAACACAYAAADDPmHLAAACV0lEQVR4nO3dPUrDYByA8UQ8g15AI+gsOOnmrufoIBT0DAUFB+/R3bFTobOCwQvoJSouNcObhHyZ9n2eHwiirW3Th79J2iaJJEmSJEmSJIC06iGu1+vgz9M0Df9CY6t8PkP2fMrYDADOAOAMAM4A4OrWGl3bj0Pp8+wEgDMAuP2uD//w7I6+DEf19fbc6eadAHAGAGcAcAYAZwBwnbcCTrIj+jL8Fx/55yA34wSAMwA4A4AzADgDgDMAOAOAMwC4zjuCzi+uN9+fZgeNrvuefw+69FfL10H/fgycAHAGAGcAcAYAZwBwnbcCioZeq2+quIVS5NbBHycAnAHARffRsOksr71Ml38Bi/mk9XVH5EfDFGYAcHVbAWWjw08NbyePEaRmDADOAOAMAM4A4Fq9FjCd5cG1zaeHrPeleXnzsvl+MZ802vooe4fSatn9ftUILp/iYxlCm51UTgA4A4Dr9eXgsv3wtJdfhx71fXICwBkAXGUAv+cLCH0pHk4AOAOAMwA4A4AzALhedwRpXBVneSu9X04AOAOAMwA4A4AzADgDgDMAOAOAMwA4A4AzADgDgDMAOAOAMwA4A4AzALio3xG0bUcu3UZOADgDgDMAOAOAMwC4qLcCRjxG0M5wAsAZAJwBwBkAnAHAGQCcAcAZAJwBwBkAnAHA+Y4gOCcAnAHAGQCcAcAZAFyrrYDH++NGl7+6ZZ0yZpc4AeAMAC66HUFDnLwyZk4AOAOAKz+QfMXx58dScdz7se5o8A7t0HJzAtAZAJwBwBkAnAFIkiRJkiRJUtySJPkBweNXgRaWkYQAAAAASUVORK5CYII=)](https://github.com/topics/skribbl-typo)  

This microservice implements **Discord Linked Role Metadata** in .NET .  
It exposes:  
- A gRPC service which is used internally from other services to trigger user metadata update events  
- A REST API which implements endpoints for Discord OAuth and metadata management
- A database for storing tokens

Functionality is split in service classes:
- A service for managing and pushing metadata definition and user metadata to/from discord
- A service for managing OAuth2 login and token refresh
- A service to build metadata from palantir user details
- A service to register the metadata scheme on discord

## Metadata registration
On startup, a hosted service gets the metadata scheme (a static property in Util) and pushes it to discord.  
Via Swagger (in development /swagger/index.html) the current metadata scheme can be checked.

## User connection
When users want to verify linked roles, they are redirected to the URL specified in the discord developer dashboard.  
They land at the REST /connect, where a session cookie is set, and are redirected to the discord OAuth login.  
Upon successful authorization, the discord OAuth redirects the user to /connect-callback which trades the code grant to an access & refresh token, which are saved in the databaase so that the app can automatically update user metadata.  
The app also builds the current metadata and pushes it to discord, so that the user can aquire the role right now.  

## User metadata update
The gRPC server enables other local services to trigger metadata update for users.  
The service receives a list of discord Id, for which it checks if a discord token is stored.  
If no token is stored, the user has not connected to linked roles. If the token is stored, but expired, it is refreshed.  
This might be inperformant when a large number of ids is passed, since DB actions must not be executed on the same DbContext in parallel.  
When the tokens are fetched, the metadata for the users is built and pushed in parallel.
