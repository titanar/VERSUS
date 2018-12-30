import React from 'react';
import ReactDOM from 'react-dom';
import ReactDOMServer from 'react-dom/server';

import SiteTest from './SiteTest.jsx';

global.React = React;
global.ReactDOM = ReactDOM;
global.ReactDOMServer = ReactDOMServer;

// React server-side rendering
global.SiteTest = SiteTest;

require('expose-loader?ProductScrollSection!./ProductScrollSection.js');