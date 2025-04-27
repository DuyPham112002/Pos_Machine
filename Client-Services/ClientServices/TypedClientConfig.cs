﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_POS_Services.ClientServices
{
    public interface ITypedClientConfig
    {
        Uri BaseUrl { get; set; }

        int Timeout { get; set; }
    }
    public class TypedClientConfig : ITypedClientConfig
    {
        public TypedClientConfig(IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (Uri.TryCreate(configuration["TypedClient:BaseUrl"], UriKind.Absolute, out Uri uri))
                this.BaseUrl = uri;
            else throw new ArgumentNullException("Convert uri fail!");

            if (int.TryParse(configuration["TypedClient:Timeout"], out int timeout))
                this.Timeout = timeout;
            else throw new ArgumentNullException("Convert timeout fail!");
        }

        public Uri BaseUrl { get; set; }

        public int Timeout { get; set; }
    }
}
