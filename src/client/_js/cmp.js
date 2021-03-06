(function(window, document) {
  if (!window.__cmp) {
    window.__cmp = (function() {
      var listen = window.attachEvent || window.addEventListener;
      listen('message', function(event) {
        window.__cmp.receiveMessage(event);
      }, false);

      function addLocatorFrame() {
        if (!window.frames['__cmpLocator']) {
          if (document.body) {
            var frame = document.createElement('iframe');
            frame.style.display = 'none';
            frame.name = '__cmpLocator';
            document.body.appendChild(frame);
          } else {
            setTimeout(addLocatorFrame, 5);
          }
        }
      }
      addLocatorFrame();

      var commandQueue = [];
      var cmp = function(command, parameter, callback) {
        if (command === 'ping') {
          if (callback) {
            callback({
              gdprAppliesGlobally: !!(window.__cmp && window.__cmp.config && window.__cmp.config.gdprAppliesGlobally),
              cmpLoaded: false
            });
          }
        } else {
          commandQueue.push({
            command: command,
            parameter: parameter,
            callback: callback
          });
        }
      }
      cmp.commandQueue = commandQueue;
      cmp.receiveMessage = function(event) {
        var data = event && event.data && event.data.__cmpCall;
        if (data) {
          commandQueue.push({
            callId: data.callId,
            command: data.command,
            parameter: data.parameter,
            event: event
          });
        }
      };
      cmp.config = {
        //
        // Modify config values here
        //
        globalVendorListLocation: '_api/config/vendors.json',
        globalConsentLocation: '/portal.html',
        customPurposeListLocation: '_api/config/purposes.json',
        storeConsentGlobally: true,
        storePublisherData: true,
        logging: 'debug',
        allowedVendorIds: [1, 2, 3]
      }
      return cmp;
    }());
    var t = document.createElement('script');
    t.async = false;
    t.src = 'https://acdn.origin.appnexus.net/cmp/cmp.bundle.js';
    var tag = document.getElementsByTagName('head')[0];
    tag.appendChild(t);
  }
})(window, document);
