import moment, { Duration } from 'moment';
import React, { FC, useCallback, useEffect, useMemo, useState } from 'react';
import wretch from 'wretch';

import { createStyles, makeStyles } from '@material-ui/styles';

import { kenticoKontent } from '../../appSettings.json';
import { element } from '../../terms.en-us.json';
import { loadModule } from '../../utilities/modules';
import { Loading } from '../Loading';
import { IDeepCloneConfig } from '../shared/IDeepCloneConfig';
import { IDeepCloneResponse } from '../shared/IDeepCloneResponse';
import { IContext } from '../shared/models/customElement/IContext';
import { ICustomElement } from '../shared/models/customElement/ICustomElement';
import { IContentItem } from '../shared/models/management/IContentItem';

// Expose access to Kentico custom element API
declare const CustomElement: ICustomElement<IDeepCloneConfig>;

const useStyles = makeStyles(() =>
  createStyles({
    row: { display: 'flex', flexDirection: 'row' },
    fullWidthCell: { flex: 1 },
  })
);

export const DeepClone: FC = () => {
  const styles = useStyles();

  const [available, setAvailable] = useState(false);
  const [enabled, setEnabled] = useState(true);
  const [customElementConfig, setCustomElementConfig] = useState<IDeepCloneConfig | null>(null);
  const [customElementContext, setCustomElementContext] = useState<IContext | null>(null);

  const [loading, setLoading] = useState(false);
  const [loaded, setLoaded] = useState(false);
  const [totalApiCalls, setTotalApiCalls] = useState(0);
  const [totalTime, setTotalTime] = useState<Duration>();
  const [newItems, setNewItems] = useState<IContentItem[]>([]);
  const [error, setError] = useState<string>();

  useEffect(() => {
    if (!available) {
      const initCustomElement = (element: ICustomElement<IDeepCloneConfig>, context: IContext) => {
        setAvailable(true);
        setElementEnabled(!element.disabled);
        setCustomElementConfig(element.config);
        setCustomElementContext(context);

        CustomElement.onDisabledChanged((disabled) => setElementEnabled(!disabled));
      };

      const setElementEnabled = (enabled: boolean) => {
        setEnabled(enabled);
      };

      loadModule(kenticoKontent.customElementScriptEndpoint, () => CustomElement.init(initCustomElement));
    }
  }, [available]);

  useEffect(() => {
    if (available) {
      CustomElement.setHeight(document.documentElement.scrollHeight);
    }
  });

  const cloneElement = useCallback(async () => {
    if (customElementConfig && customElementContext) {
      setLoading(true);
      setLoaded(false);

      const request = wretch(
        `${customElementConfig.backendEndpoint}/${customElementContext.item.codename}/${customElementContext.variant.codename}`
      )
        .post()
        .json<IDeepCloneResponse>();

      try {
        const response = await request;

        setTotalApiCalls(response.totalApiCalls);
        setTotalTime(moment.duration(response.totalMilliseconds));
        setNewItems(response.newItems);
      } catch (error) {
        setError(error.message);
      }

      setLoading(false);
      setLoaded(true);
    }
  }, [customElementConfig, customElementContext]);

  const getTotalTimeString = useMemo(() => {
    if (totalTime) {
      let result = [];

      if (totalTime.hours() > 0) {
        result.push(`${totalTime.hours()} ${element.time.hours}`);
      }

      if (totalTime.minutes() > 0) {
        result.push(`${totalTime.minutes()} ${element.time.minutes}`);
      }

      if (totalTime.seconds() > 0) {
        result.push(`${totalTime.seconds() + totalTime.milliseconds() / 1000} ${element.time.seconds}`);
      }

      return result.join(', ');
    }
  }, [totalTime]);

  return (
    <div>
      {loading && <Loading />}
      {error && <div>{error}</div>}
      {error === undefined && available && enabled && (
        <>
          <div className={styles.row}>
            <div className={styles.fullWidthCell}>
              <p>{element.enabledDescription}</p>
            </div>
            <div>
              <button className='btn btn--primary btn--xs' onClick={cloneElement}>
                {element.button}
              </button>
            </div>
          </div>
          {loaded && customElementContext && totalTime && (
            <>
              <div className={styles.row}>
                <div className='content-item-element__name-status'>
                  <label className='content-item-element__label'>{element.totalTime}</label>
                  <p>{getTotalTimeString}</p>
                </div>
              </div>
              <div className={styles.row}>
                <div className='content-item-element__name-status'>
                  <label className='content-item-element__label'>{element.totalApiCalls}</label>
                  <p>{totalApiCalls}</p>
                </div>
              </div>
              <div className={styles.row}>
                <div className='content-item-element__name-status'>
                  <label className='content-item-element__label'>{element.newItems}</label>
                  {newItems.map((item) => (
                    <p key={item.id}>
                      <a
                        href={`https://app.kontent.ai/${customElementContext.projectId}/content-inventory/${customElementContext.variant.id}/content/${item.id}`}
                        target='_blank'
                        rel='noopener noreferrer'
                      >
                        {item.name}
                      </a>
                    </p>
                  ))}
                </div>
              </div>
            </>
          )}
        </>
      )}
      {error === undefined && !enabled && (
        <div className='content-item-element__guidelines'>
          <p>{element.disabledDescription}</p>
        </div>
      )}
    </div>
  );
};
