
import {EventReceivedPlugin} from "./EventReceived.js";
import {DriversPlugin} from "./OutcomesDriver.js";

const elsaStudioRoot = document.querySelector('elsa-studio-root');

elsaStudioRoot.addEventListener('initializing', e => {
  const elsaStudio = e.detail;
  elsaStudio.pluginManager.registerPlugins([EventReceivedPlugin, DriversPlugin]);
});


