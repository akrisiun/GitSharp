/*
 * Copyright (C) 2009, Stefan Schake <caytchen@gmail.com>
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

using System;
using System.Collections.Generic;
using GitSharp.Core.Transport;

namespace GitSharp.Core
{
    public class BlobBasedSubmoduleConfig : BlobBasedConfig
    {
        public BlobBasedSubmoduleConfig(Config cfg, Commit commit)
            : base(cfg, commit, ".gitmodules")
        {
        }

        public BlobBasedSubmoduleConfig(Commit commit)
            : this(null, commit)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tree">Root tree will be resolved so you can simply pass subtree</param>
        public BlobBasedSubmoduleConfig(Tree tree)
            : base(null, tree.Repository.OpenBlob(GetRoot(tree).FindBlobMember(".gitmodules").Id).Bytes)
        {
        }

        public int SubmoduleCount
        {
            get
            {
                return getSubsections("submodule").Count;
            }
        }

        public SubmoduleEntry GetEntry(int index)
        {
            string name = getSubsections("submodule")[index];
            return CreateEntry(name);
        }

        public IEnumerable<SubmoduleEntry> GetEntries()
        {
            foreach (string name in getSubsections("submodule"))
                yield return CreateEntry(name);
        }

        static Tree GetRoot(Tree tree)
        {
            if (tree == null) throw new ArgumentNullException("tree");
            Tree parentTree2 = tree;
            while (parentTree2.Parent != null)
                parentTree2 = parentTree2.Parent;
            return parentTree2;
        }

        SubmoduleEntry CreateEntry(string name)
        {
            string path = getString("submodule", name, "path");
            string url = getString("submodule", name, "url");
            string update = getString("submodule", name, "update");

            SubmoduleEntry.UpdateMethod method;
            switch (update)
            {
                case "rebase":
                    method = SubmoduleEntry.UpdateMethod.Rebase;
                    break;

                case "merge":
                    method = SubmoduleEntry.UpdateMethod.Merge;
                    break;

                default:
                    method = SubmoduleEntry.UpdateMethod.Checkout;
                    break;
            }

            return new SubmoduleEntry(name, path, url, method);
        }
    }
}