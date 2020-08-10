import React, { FC } from 'react';

import { errors } from '../terms.en-us.json';

interface IErrorProps {
  message?: string;
  stack?: string;
}

export const Error: FC<IErrorProps> = ({ message, stack }) => {
  let errorMessage: IErrorProps = { message: errors.genericError, stack: errors.genericStack };

  message && (errorMessage.message = message);
  stack && (errorMessage.stack = stack);

  return (
    <>
      <h1>{errorMessage.message}</h1>
      <span>{errorMessage.stack}</span>
    </>
  );
};
