import React from 'react';

import { createStyles, makeStyles } from '@material-ui/styles';

import { kenticoKontent } from '../appSettings.json';
import { shared } from '../terms.en-us.json';

const useStyles = makeStyles(() =>
  createStyles({
    root: { margin: '1em' },
  })
);

export const InvalidUsage = () => {
  const styles = useStyles();

  return (
    <div className={styles.root}>
      <span>
        {shared.pleaseRead} <a href={kenticoKontent.documentationUrl}>{shared.whatToDo}</a>.
      </span>
    </div>
  );
};
