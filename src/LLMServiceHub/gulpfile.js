/// <binding ProjectOpened='watchJsSrc, watchJsPage, watchCssbundle' />
'use strict';

import { deleteAsync } from 'del';
import gulp from 'gulp';
import babel from 'gulp-babel';
import concat from 'gulp-concat';
import * as dartSass from 'sass';
import gulpSass from 'gulp-sass';
const sass = gulpSass(dartSass);

import minifyCSS from 'gulp-clean-css';
import rename from 'gulp-rename';
import uglify from 'gulp-uglify';



//const _assets_folder = './Assets';
const _npm_folder = './node_modules';
const _lib_folder = './wwwroot/lib';
const _dist_folder = './wwwroot/dist';
const _src_folder = './frontend_src';
const _assets_folder = './Assets';
const commentsPattern = '/(^!)|(@cc_on)|(@preserve)|(@license)/';

// 写日志
function logInfo(msg) {
    let now = new Date().toLocaleTimeString();
    console.log(`[${now}] ${msg}`);
}

// 压缩css方法
function cssmin() {
    return minifyCSS({ sourceMap: true, debug: true }, (details) => {
        logInfo(`${details.name}: ${details.stats.originalSize} -> ${details.stats.minifiedSize}`);
    });
}

async function cleanAll() {
    return await deleteAsync(`${_dist_folder}/**`, { force: true });
}

function css() {
    logInfo('[css] Script start');
    return gulp
        .src(`${_src_folder}/Styles/style.scss`, { allowEmpty: true })
        .pipe(sass().on('error', sass.logError))
        .pipe(concat('style.bundle.css'))
        .pipe(gulp.dest(_dist_folder + '/css'))
        .pipe(cssmin())
        //.pipe(concat('style.min.css'))
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest(_dist_folder + '/css'));
}

// 单页js
function jsPerpage() {
    logInfo('[jsPerpage] Script start');
    let outputfolder = `${_dist_folder}/js/pages`;
    return gulp.src(`${_src_folder}/Scripts/pages/**/*.js`, { allowEmpty: true })
        // .pipe(webpack({
        //   entry: {
        //     index: ['./Scripts/pages/index/index.js'],
        //     user: ['./Scripts/pages/user/user.js','./Scripts/pages/user/user.app.js']
        //   },
        //   output: {
        //     filename: '[name].min.js',
        //   },
        // }))
        .pipe(babel({ compact: false, comments: true }))
        .pipe(gulp.dest(outputfolder))
        .pipe(uglify({
            output: {
                ascii_only: true,
                comments: commentsPattern
            }
        }))
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest(outputfolder));
}

// 直接拷贝的js库
function procLib(done) {
    logInfo('[procLib] Script start');
    let libs = {
        "@tabler/core": {
            from: [`${_npm_folder}/@tabler/core/dist/**`]
        },
        "@tabler/core/css": {
            from: [
                `${_npm_folder}/@tabler/core/dist/css/tabler.min.css`,
                `${_npm_folder}/@tabler/core/dist/css/tabler-flags.min.css`,
                `${_npm_folder}/@tabler/core/dist/css/tabler-payments.min.css`,
                `${_npm_folder}/@tabler/core/dist/css/tabler-vendors.min.css`
            ],
            bundle: true,
            dest: '@tabler/core/css',
            name: 'tabler-style-bundle',
            ext: '.css'
        },
        "@tabler/icons-webfont": {
            from: [`${_npm_folder}/@tabler/icons-webfont/**`]
        },
        "@fortawesome/fontawesome-free": {
            from: [`${_npm_folder}/@fortawesome/fontawesome-free/**`]
        },
        "bootstrap": {
            from: [`${_npm_folder}/bootstrap/dist/**`]
        },
        "jquery": {
            from: [`${_npm_folder}/jquery/dist/**`]
        },
        "marked": {
            from: [`${_npm_folder}/marked/**`]
        },
        "easy-typer-js": {
            from: [`${_npm_folder}/easy-typer-js/**`]
        }
    };
    let _libtasks = Object.keys(libs);
    _libtasks.forEach(function (n) {
        logInfo(`copy library ${n} ...`);
        (function () {
            let o = libs[n];
            let _d = {
                bundle: false,
                babel: false,
                uglify: false,
            };
            let lib = {
                ..._d,
                ...o
            };
            console.log(lib);
            logInfo(lib.from + ' -> ' + _lib_folder + '/' + n);
            let stream = gulp.src(lib.from, { allowEmpty: true })
                .on('error', function (err) {
                    gutil.log(gutil.colors.red('[Error]'), err.toString());
                });

            if (lib.bundle) {
                logInfo(`!!!! bundle custom: ${lib.dest}/${lib.name}`);
                stream = stream.pipe(concat(lib.name + lib.ext))
                    .pipe(gulp.dest(_lib_folder + '/' + lib.dest));
            }
            else {
                stream = stream.pipe(gulp.dest(_lib_folder + '/' + n));
            }
            return stream;
        })();
    });
    done();
}

// 需要构建打包的js
function jsSrc(done) {
    logInfo('[jsSrc] Script start');
    let components = {
        "app-core": {
            src: [
                //`${_npm_folder}/@tabler/`,
                `${_src_folder}/Scripts/components/GlobalUtility.js`,
                `${_src_folder}/Scripts/components/AppWebComponents.js`,
                `${_src_folder}/Scripts/components/Toast.js`,
            ],
            dist_sub: '/js/app-core',
            name: 'app-core'
        },
        "upload": {
            src: [
                //`${_npm_folder}/crypto-js/crypto-js.js`,
                //`${_npm_folder}/ali-oss/dist/aliyun-oss-sdk.js`,
                `${_npm_folder}/bootstrap-fileinput/js/fileinput.js`,
                `${_npm_folder}/bootstrap-fileinput/themes/bs5/theme.js`,
                `${_npm_folder}/bootstrap-fileinput/themes/fa5/theme.js`,
                `${_npm_folder}/bootstrap-fileinput/js/locales/zh.js`,
                `${_src_folder}/Scripts/components/upload/ossClient.js`,
                `${_src_folder}/Scripts/components/upload/uploadUtil.js`,
                `${_src_folder}/Scripts/components/upload/fileUploadControl.js`,
                `${_src_folder}/Scripts/components/multipleinput/multipleinput.js`,
            ],
            dist_sub: '/js/upload',
            name: 'oss-upload-util'
        },
        "home": {
            src: [`${_src_folder}/Scripts/pages/home/*.js`],
            dist_sub: '/js/pages',
            name: 'home'
        },
        "user": {
            src: [
                `${_src_folder}/Scripts/pages/user/user.js`,
                `${_src_folder}/Scripts/pages/user/user.app.js`],
            dist_sub: '/js/pages',
            name: 'user'
        },
        "bsConfirm": {
            src: [
                `${_src_folder}/Scripts/components/bsConfirm/bsConfirm.js`,
            ],
            dist_sub: '/js/util',
            name: 'bsConfirm'
        }
    };
    let _tasks = Object.keys(components);
    _tasks.forEach(function (n) {
        logInfo(`bundling ${n} ...`);
        (function () {
            let item = components[n];
            let useBabel = item.babel || true;
            let __g = gulp.src(item.src, { allowEmpty: true })
                .on('error', function (err) {
                    gutil.log(gutil.colors.red('[Error]'), err.toString());
                })
                .pipe(concat((item.name || n) + '.js'));
            if (useBabel) {
                __g = __g.pipe(babel({ compact: false, comments: true }));
            }
            __g = __g
                .pipe(gulp.dest(_dist_folder + item.dist_sub))
                .pipe(uglify({
                    output: {
                        ascii_only: true,
                        comments: commentsPattern
                    }
                }))
                .pipe(rename({ suffix: '.min' }))
                .pipe(gulp.dest(_dist_folder + item.dist_sub));
            return __g;
        })();
    });
    done();
}

// 打包任务

let _clean = gulp.parallel([cleanAll]);
let _build = gulp.series([procLib]);


//gulp.task('clean', _clean);
gulp.task('build', _build);


export default _build;