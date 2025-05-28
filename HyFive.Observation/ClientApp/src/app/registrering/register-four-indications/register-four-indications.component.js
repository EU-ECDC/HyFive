"use strict";
var __esDecorate = (this && this.__esDecorate) || function (ctor, descriptorIn, decorators, contextIn, initializers, extraInitializers) {
    function accept(f) { if (f !== void 0 && typeof f !== "function") throw new TypeError("Function expected"); return f; }
    var kind = contextIn.kind, key = kind === "getter" ? "get" : kind === "setter" ? "set" : "value";
    var target = !descriptorIn && ctor ? contextIn["static"] ? ctor : ctor.prototype : null;
    var descriptor = descriptorIn || (target ? Object.getOwnPropertyDescriptor(target, contextIn.name) : {});
    var _, done = false;
    for (var i = decorators.length - 1; i >= 0; i--) {
        var context = {};
        for (var p in contextIn) context[p] = p === "access" ? {} : contextIn[p];
        for (var p in contextIn.access) context.access[p] = contextIn.access[p];
        context.addInitializer = function (f) { if (done) throw new TypeError("Cannot add initializers after decoration has completed"); extraInitializers.push(accept(f || null)); };
        var result = (0, decorators[i])(kind === "accessor" ? { get: descriptor.get, set: descriptor.set } : descriptor[key], context);
        if (kind === "accessor") {
            if (result === void 0) continue;
            if (result === null || typeof result !== "object") throw new TypeError("Object expected");
            if (_ = accept(result.get)) descriptor.get = _;
            if (_ = accept(result.set)) descriptor.set = _;
            if (_ = accept(result.init)) initializers.unshift(_);
        }
        else if (_ = accept(result)) {
            if (kind === "field") initializers.unshift(_);
            else descriptor[key] = _;
        }
    }
    if (target) Object.defineProperty(target, contextIn.name, descriptor);
    done = true;
};
var __runInitializers = (this && this.__runInitializers) || function (thisArg, initializers, value) {
    var useValue = arguments.length > 2;
    for (var i = 0; i < initializers.length; i++) {
        value = useValue ? initializers[i].call(thisArg, value) : initializers[i].call(thisArg);
    }
    return useValue ? value : void 0;
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g = Object.create((typeof Iterator === "function" ? Iterator : Object).prototype);
    return g.next = verb(0), g["throw"] = verb(1), g["return"] = verb(2), typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (g && (g = 0, op[0] && (_ = 0)), _) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
var __setFunctionName = (this && this.__setFunctionName) || function (f, name, prefix) {
    if (typeof name === "symbol") name = name.description ? "[".concat(name.description, "]") : "";
    return Object.defineProperty(f, "name", { configurable: true, value: prefix ? "".concat(prefix, " ", name) : name });
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.RegisterFourIndicationsComponent = void 0;
var core_1 = require("@angular/core");
var queryparameters_1 = require("../../constants/queryparameters");
var uuid_1 = require("../../utils/uuid");
var free_solid_svg_icons_1 = require("@fortawesome/free-solid-svg-icons");
var urls_1 = require("../../constants/urls");
var RegisterFourIndicationsComponent = function () {
    var _classDecorators = [(0, core_1.Component)({
            selector: 'app-register-four-indications',
            templateUrl: './register-four-indications.component.html',
        })];
    var _classDescriptor;
    var _classExtraInitializers = [];
    var _classThis;
    var RegisterFourIndicationsComponent = _classThis = /** @class */ (function () {
        function RegisterFourIndicationsComponent_1(sessionService, router, route, institutionService, toastrService) {
            var _this = this;
            this.sessionService = sessionService;
            this.router = router;
            this.route = route;
            this.institutionService = institutionService;
            this.toastrService = toastrService;
            this.Urls = urls_1.Urls;
            this.sessionsdata = null;
            this.showRoleList = false;
            this.showEmptyForShortText = false;
            this.faPlus = free_solid_svg_icons_1.faPlus;
            this.faCircle = free_solid_svg_icons_1.faCircle;
            this.institutionService
                .getSelectedInstitution()
                .subscribe(function (i) { var _a; return _this.roles = (_a = i.departments.find(function (a) { var _a; return a.id === ((_a = _this.sessionView.department) === null || _a === void 0 ? void 0 : _a.id); })) === null || _a === void 0 ? void 0 : _a.roles; });
        }
        RegisterFourIndicationsComponent_1.prototype.ngOnInit = function () {
            var _this = this;
            var _a;
            this.route
                .queryParams
                .subscribe(function (params) {
                var sessionId = params[queryparameters_1.Queryparameters.SessionId] || 0;
                _this.sessionView = _this.sessionService.getSessionViewForSession(sessionId);
                if (!_this.sessionView)
                    _this.router.navigate(['']);
                else
                    _this.loadSessionData();
            });
            if (((_a = this.sessionView.card) === null || _a === void 0 ? void 0 : _a.length) === 0)
                this.showEmptyForShortText = true;
        };
        RegisterFourIndicationsComponent_1.prototype.ngOnDestroy = function () {
            this.toastrService.clear();
        };
        RegisterFourIndicationsComponent_1.prototype.registerObservation = function (observation) {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, this.sessionService.registerObservation(observation)];
                        case 1:
                            _a.sent();
                            this.toastrService.success("Observation was saved");
                            this.loadSessionData();
                            return [2 /*return*/];
                    }
                });
            });
        };
        RegisterFourIndicationsComponent_1.prototype.loadSessionData = function () {
            this.sessionsdata = this.sessionService.getSession(this.sessionView.sessionId);
        };
        RegisterFourIndicationsComponent_1.prototype.toggleRoleList = function () {
            this.showRoleList = !this.showRoleList;
        };
        RegisterFourIndicationsComponent_1.prototype.addNewCard = function (role) {
            this.sessionView.card = this.sessionView.card.map(function (k) { k.isActive = false; return k; });
            this.sessionView.card.push({ id: uuid_1.Uuid.generateUUID(), role: role, isActive: true });
            this.updateSessionView(this.sessionView);
            this.toggleRoleList();
        };
        RegisterFourIndicationsComponent_1.prototype.updateSessionView = function (sessionView) {
            var _a;
            this.sessionView = this.sessionService.updateSessionViewForSession(sessionView);
            if (((_a = this.sessionView.card) === null || _a === void 0 ? void 0 : _a.length) === 0)
                this.showEmptyForShortText = true;
            else
                this.showEmptyForShortText = false;
        };
        RegisterFourIndicationsComponent_1.prototype.cardIsSelected = function (selectedCard) {
            for (var i = 0; i < this.sessionView.card.length; i++) {
                if (this.sessionView.card[i] != selectedCard) {
                    this.sessionView.card[i].isActive = false;
                }
            }
            this.sessionService.updateSessionViewForSession(this.sessionView);
        };
        RegisterFourIndicationsComponent_1.prototype.onCloseNewCardModal = function (result) {
            var _this = this;
            if (result) {
                result.forEach(function (x) { return _this.addNewCard(x); });
            }
        };
        RegisterFourIndicationsComponent_1.prototype.onDismissNewShortModal = function (reason) {
        };
        return RegisterFourIndicationsComponent_1;
    }());
    __setFunctionName(_classThis, "RegisterFourIndicationsComponent");
    (function () {
        var _metadata = typeof Symbol === "function" && Symbol.metadata ? Object.create(null) : void 0;
        __esDecorate(null, _classDescriptor = { value: _classThis }, _classDecorators, { kind: "class", name: _classThis.name, metadata: _metadata }, null, _classExtraInitializers);
        RegisterFourIndicationsComponent = _classThis = _classDescriptor.value;
        if (_metadata) Object.defineProperty(_classThis, Symbol.metadata, { enumerable: true, configurable: true, writable: true, value: _metadata });
        __runInitializers(_classThis, _classExtraInitializers);
    })();
    return RegisterFourIndicationsComponent = _classThis;
}();
exports.RegisterFourIndicationsComponent = RegisterFourIndicationsComponent;
//# sourceMappingURL=register-four-indications.component.js.map