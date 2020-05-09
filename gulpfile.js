const gulp = require('gulp');
const concat = require('gulp-concat');
const header = require('gulp-header');
const modify = require('gulp-modify-file');

const headers = [];

function unique(value, index, self) { 
    return self.indexOf(value) === index;
}

gulp.task('compile', function () {
    return gulp.src([
        './src/*.cs',
        './src/common/**/*.cs',
        './src/bot/**/*.cs'
    ])
        .pipe(modify((content, filePath, file) => {

            const regex = /using.*\n/g;

            const imports = content.match(regex);

            if(imports)
                headers.push(...imports);
            
            return content.replace(regex, "");
        }))
        .pipe(concat('Build.cs'))
        .pipe(gulp.dest('./build/'));
});

gulp.task('header', function () {
    return gulp.src(['./build/Build.cs'])
        .pipe(header(headers.filter(unique).join('')))
        .pipe(gulp.dest('./build/'));
});

gulp.task('default', gulp.series('compile', 'header'));