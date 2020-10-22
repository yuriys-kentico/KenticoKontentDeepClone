import React, { FC } from 'react';

import { createStyles, makeStyles } from '@material-ui/styles';

import { errors } from '../terms.en-us.json';

interface IErrorProps {
  message?: string;
  stack?: string;
}

const useStyles = makeStyles(() =>
  createStyles({
    span: { whiteSpace: 'pre' },
  })
);

export const Error: FC<IErrorProps> = ({ message, stack }) => {
  const styles = useStyles();

  let errorMessage: IErrorProps = { message: errors.genericError, stack: errors.genericStack };

  message && (errorMessage.message = message);
  stack && (errorMessage.stack = stack);

  return (
    <>
      <h1>{errorMessage.message}</h1>
      <span className={styles.span}>{errorMessage.stack}</span>
    </>
  );
};
