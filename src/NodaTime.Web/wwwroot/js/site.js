// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

// Initialise Foundation.
$(document).foundation();

// Load the NuGet button.
(function () {
  var nb = document.createElement('script'); nb.type = 'text/javascript'; nb.async = true;
  nb.src = 'http://s.prabir.me/nuget-button/0.2.1/nuget-button.min.js';
  var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(nb, s);
})();

// Initialise highlight.js.
hljs.initHighlightingOnLoad();
