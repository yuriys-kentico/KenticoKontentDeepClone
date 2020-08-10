import './styles/style.scss';
import 'react-app-polyfill/ie11';
import 'react-app-polyfill/stable';
import 'typeface-source-sans-pro';

import React from 'react';
import { render } from 'react-dom';

import { App } from './app/App';

render(<App />, document.getElementById('root'));
