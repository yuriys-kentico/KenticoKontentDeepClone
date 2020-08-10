import React from 'react';

import { kenticoKontent } from '../appSettings.json';

export const InvalidUsage = () => (
  <div className='invalidUsage'>
    <span>
      Please read this: <a href={kenticoKontent.documentationUrl}>What to do with the URL to this page</a>.
    </span>
  </div>
);
