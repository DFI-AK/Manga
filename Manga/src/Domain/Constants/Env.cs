﻿using Microsoft.AspNetCore.Identity;

namespace Manga.Domain.Constants;
public class Env
{
    public const string MangaDb = nameof(MangaDb);
    public const string CorsPolicy = nameof(CorsPolicy);
    public const string AccessURLs = nameof(AccessURLs);
    public const string AuthorizeSchema = "Identity.Bearer";
}
