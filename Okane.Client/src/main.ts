import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import { registerInstrumentations } from '@opentelemetry/instrumentation';
import { FetchInstrumentation } from '@opentelemetry/instrumentation-fetch';
import { WebTracerProvider } from '@opentelemetry/sdk-trace-web';

new WebTracerProvider().register();

registerInstrumentations({
  instrumentations: [
    new FetchInstrumentation({
      propagateTraceHeaderCorsUrls: [
        /http:\/\/localhost:5091\/api\/.*/
      ]
    })
  ]
});

bootstrapApplication(App, appConfig)
  .catch((err) => console.error(err));
