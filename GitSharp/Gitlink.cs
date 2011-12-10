/*
 * Copyright (C) 2011, Linquize <linquize@yahoo.com.hk>
 *
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or
 * without modification, are permitted provided that the following
 * conditions are met:
 *
 * - Redistributions of source code must retain the above copyright
 *   notice, this list of conditions and the following disclaimer.
 *
 * - Redistributions in binary form must reproduce the above
 *   copyright notice, this list of conditions and the following
 *   disclaimer in the documentation and/or other materials provided
 *   with the distribution.
 *
 * - Neither the name of the Git Development Community nor the
 *   names of its contributors may be used to endorse or promote
 *   products derived from this software without specific prior
 *   written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System.Linq;
using CoreGitLink = GitSharp.Core.GitLinkTreeEntry;
using CoreSubmoduleConfig = GitSharp.Core.BlobBasedSubmoduleConfig;

namespace GitSharp
{
    public class Gitlink
    {
        internal Gitlink(Repository repo, CoreGitLink link)
        {
            _repo = repo;
            _internal_link = link;
        }

        CoreGitLink _internal_link;
        Repository _repo;

        /// <summary>
        /// The git object's SHA1 hash. This is the long hash, See ShortHash for the abbreviated version.
        /// </summary>
        public string Hash
        {
            get
            {
                if (_internal_link == null || _internal_link.Id == null)
                    return null;
                return _internal_link.Id.Name;
            }
        }

        /// <summary>
        /// The file name
        /// </summary>
        public string Name
        {
            get { return _internal_link.Name; }
        }

        /// <summary>
        /// The full path relative to repostiory root
        /// </summary>
        public string Path
        {
            get { return _internal_link.FullName; }
        }

        /// <summary>
        /// The unix file permissions.
        /// 
        /// Todo: model this with a permission object
        /// </summary>
        public int Permissions
        {
            get { return _internal_link.Mode.Bits; }
        }

        /// <summary>
        /// The parent <see cref="Tree"/>.
        /// </summary>
        public Tree Parent
        {
            get { return new Tree(_repo, _internal_link.Parent); }
        }

        public string Url
        {
            get
            {
                var tree = _internal_link.Parent;
                var config = new CoreSubmoduleConfig(_internal_link.Parent);
                var entry = config.GetEntries().FirstOrDefault(a => a.Path == this.Path);
                return entry != null && entry.Url != null ? entry.Url.Path : null;
            }
        }
    }
}
