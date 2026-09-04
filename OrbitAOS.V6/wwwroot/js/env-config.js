/**
 * env-config.js
 *
 * Runtime environment configuration for containerized deployments.
 * This file is intended to be replaced or overridden via Kubernetes ConfigMap
 * mounted at /app/wwwroot/js/env-config.js so that environment-specific values
 * can be injected at container startup without rebuilding the image.
 *
 * Kubernetes ConfigMap example:
 *   apiVersion: v1
 *   kind: ConfigMap
 *   metadata:
 *     name: orbitaos-env-config
 *   data:
 *     env-config.js: |
 *       window.__env = {
 *         ARROW_LEFT_KEY: 'ArrowLeft',
 *         ARROW_RIGHT_KEY: 'ArrowRight',
 *         SPACE_KEY: 'Space',
 *         ARROW_UP_KEY: 'ArrowUp',
 *         ARROW_DOWN_KEY: 'ArrowDown',
 *         TAB_KEY: 'Tab',
 *         ESCAPE_KEY: 'Escape',
 *         DATA_API_KEY: '.data-api',
 *         BS_TOAST_DATA_KEY: 'bs.toast'
 *       };
 *
 * Rule: cz-js-1057 - Hardcoded Environment Values in jQuery/Bootstrap Code
 * Remediation: Externalize hardcoded values via Kubernetes ConfigMaps on EKS
 */
window.__env = window.__env || {};

// Bootstrap UI key constants - override via Kubernetes ConfigMap for environment-specific values
window.__env.ARROW_LEFT_KEY = window.__env.ARROW_LEFT_KEY || 'ArrowLeft';
window.__env.ARROW_RIGHT_KEY = window.__env.ARROW_RIGHT_KEY || 'ArrowRight';
window.__env.SPACE_KEY = window.__env.SPACE_KEY || 'Space';
window.__env.ARROW_UP_KEY = window.__env.ARROW_UP_KEY || 'ArrowUp';
window.__env.ARROW_DOWN_KEY = window.__env.ARROW_DOWN_KEY || 'ArrowDown';
window.__env.TAB_KEY = window.__env.TAB_KEY || 'Tab';
window.__env.ESCAPE_KEY = window.__env.ESCAPE_KEY || 'Escape';
window.__env.DATA_API_KEY = window.__env.DATA_API_KEY || '.data-api';
window.__env.BS_TOAST_DATA_KEY = window.__env.BS_TOAST_DATA_KEY || 'bs.toast';
